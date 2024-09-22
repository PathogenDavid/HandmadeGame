public sealed class GarbageQuestValidator : QuestValidatorBase
{
    public override PuzzleOutcome CheckPuzzle(Quest quest)
    {
        bool first = true;
        const int invalid = -1;
        int commonX = invalid;
        int commonY = invalid;
        int itemCount = 0;

        for (int x = 0; x < quest.BoardWidth; x++)
        {
            for (int y = 0; y < quest.BoardHeight; y++)
            {
                NestItem item = quest.Board[x, y];

                if (item == null)
                    continue;

                if (!item.IsGarbage)
                    return PuzzleOutcome.WrongItemsPresent;

                itemCount++;

                if (first)
                {
                    commonX = x;
                    commonY = y;
                    first = false;
                    continue;
                }

                if (commonX != x)
                    commonX = invalid;

                if (commonY != y)
                    commonY = invalid;
            }
        }

        // At this point we know if the trash is in a straight non-diagonal line, but we also need to reject trash lines which are not against the edge of the nest
        if (commonX != 0 && commonX != quest.BoardWidth - 1)
            commonX = invalid;
        if (commonY != 0 && commonY != quest.BoardHeight - 1)
            commonY = invalid;

        if (itemCount < 3)
            return PuzzleOutcome.NotEnoughItems;

        if (commonX == invalid && commonY == invalid)
            return PuzzleOutcome.IncorrectPlacement;

        return PuzzleOutcome.Perfect;
    }
}
