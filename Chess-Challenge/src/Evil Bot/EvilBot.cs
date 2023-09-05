using ChessChallenge.API;

namespace ChessChallenge.Example
{
    // A simple bot that can spot mate in one, and always captures the most
    // valuable piece it can.
    // Plays randomly otherwise.
    public class EvilBot : IChessBot
    {
        // Piece values: null, pawn, knight, bishop, rook, queen, king
        int[] pieceValues = { 0, 100, 300, 300, 500, 900, 10000 };

        public Move Think(Board board, Timer timer)
        {
            Move[] allMoves = board.GetLegalMoves();
            long score = - pieceValues[6];
            int maxDepth = 3;

            Move moveToPlay = allMoves[0];

            foreach (Move move in allMoves)
            {
                long currentScore;
                board.MakeMove(move);
                currentScore = AlphaBetaMin(
                    board,
                    long.MinValue,
                    long.MaxValue,
                    maxDepth
                );
                board.UndoMove(move);
                if (score <= currentScore)
                {
                    score = currentScore;
                    moveToPlay = move;
                }
            }

            return moveToPlay;
        }

        public long AlphaBetaMin(Board board, long alpha, long beta, int depth)
        {
            if (0 >= depth || board.IsInCheckmate() || board.IsDraw())
            {
                return - Evaluate(board);
            }
            else
            {
                foreach (Move move in board.GetLegalMoves())
                {
                    long score;
                    board.MakeMove(move);
                    score = AlphaBetaMax(board, alpha, beta, depth - 1);
                    board.UndoMove(move);
                    if (score <= alpha)
                    {
                        return alpha;
                    }
                    if (score < beta)
                    {
                        beta = score;
                    }
                }

                return beta;
            }
        }

        public long AlphaBetaMax(Board board, long alpha, long beta, int depth)
        {
            if (0 >= depth || board.IsInCheckmate() || board.IsDraw())
            {
                return Evaluate(board);
            }
            else
            {
                foreach (Move move in board.GetLegalMoves())
                {
                    long score;
                    board.MakeMove(move);
                    score = AlphaBetaMin(board, alpha, beta, depth - 1);
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

        public long Evaluate(Board board)
        {
            long score = 0;

            if (board.IsInCheckmate())
            {
                return - pieceValues[6];
            }

            foreach (PieceList pieces in board.GetAllPieceLists())
            {
                foreach (Piece piece in pieces)
                {
                    long pieceScore = pieceValues[(int)piece.PieceType];
                    if (board.IsWhiteToMove != piece.IsWhite)
                    {
                        pieceScore = - pieceScore;
                    }
                    score += pieceScore;
                }
            }

            return score;
        }
    }
}
