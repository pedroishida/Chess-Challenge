using ChessChallenge.API;
using System;

public class MyBot : IChessBot
{
    // Piece values: null, pawn, knight, bishop, rook, queen, king
    int[] pieceValues = { 0, 10, 30, 30, 50, 90, 2000 };

    public Move Think(Board board, Timer timer)
    {
        Random rng = new();
        Move[] allMoves = board.GetLegalMoves();
        int beta = - pieceValues[6] + 1;
        int alpha = beta;
        int maxDepth = 4;

        Move moveToPlay = allMoves[0];

        foreach (Move move in allMoves)
        {
            int score;
            board.MakeMove(move);
            score = - AlphaBeta(
                board,
                beta,
                - beta,
                maxDepth
            );
            board.UndoMove(move);
            if (score > alpha)
            {
                alpha = score;
                moveToPlay = move;
            }
            else if (score == alpha && 0 != rng.Next(2))
            {
                moveToPlay = move;
            }
        }

        return moveToPlay;
    }

    public int AlphaBeta(Board board, int alpha, int beta, int depth)
    {
        if (0 >= depth || board.IsInCheckmate() || board.IsDraw())
        {
            return Evaluate(board);
        }
        else
        {
            foreach (Move move in board.GetLegalMoves())
            {
                int score;
                board.MakeMove(move);
                score = - AlphaBeta(board, - beta, - alpha, depth - 1);
                board.UndoMove(move);
                if (score >= beta)
                {
                    return beta;
                }
                if (score > alpha)
                {
                    alpha = score;
                }
            }

            return alpha;
        }
    }

    public int Evaluate(Board board)
    {
        int score = 0;

        if (board.IsInCheckmate())
        {
            return - pieceValues[6];
        }

        if (board.IsDraw())
        {
            return 0;
        }

        foreach (PieceList pieces in board.GetAllPieceLists())
        {
            int pieceScore = pieceValues[(int)pieces.TypeOfPieceInList] * pieces.Count;

            if (board.IsWhiteToMove == pieces.IsWhitePieceList)
            {
                score += pieceScore;
            }
            else
            {
                score -= pieceScore;
            }
        }

        return score;
    }
}
