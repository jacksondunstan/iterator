class MonotonicRandom
{
    private int NextVal;

    public int Next()
    {
        return NextVal++;
    }

    public int Next(int max)
    {
        return Next() % max;
    }
}