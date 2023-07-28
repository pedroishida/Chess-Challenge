using ChessChallenge.API;
using System;

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

            Random rng = new();
            Move moveToPlay = allMoves[0];
            int highestValueCapture = 0;

            // Pick a random move to play that does not put the piece in danger
            int i = 0;
            do
            {
                moveToPlay = allMoves[rng.Next(allMoves.Length)];
                i++;
            } while (i < allMoves.Length &&
                board.SquareIsAttackedByOpponent(moveToPlay.TargetSquare));

            foreach (Move move in allMoves)
            {
                // Always play checkmate in one
                if (MoveIsCheckmate(board, move))
                {
                    moveToPlay = move;
                    break;
                }

                // Find highest value capture
                int capturedPieceValue = pieceValues[(int)move.CapturePieceType];
                int movingPieceValue = pieceValues[(int)move.MovePieceType];

                // Only captures into a dangerous position if target value is higher
                // than the moving piece
                if (
                    board.SquareIsAttackedByOpponent(move.TargetSquare)
                    && movingPieceValue >= capturedPieceValue
                )
                {
                    continue;
                }

                if (capturedPieceValue > highestValueCapture)
                {
                    moveToPlay = move;
                    highestValueCapture = capturedPieceValue;
                }
            }

            return moveToPlay;
        }

        // Test if this move gives checkmate
        bool MoveIsCheckmate(Board board, Move move)
        {
            board.MakeMove(move);
            bool isMate = board.IsInCheckmate();
            board.UndoMove(move);
            return isMate;
        }
    }
}
