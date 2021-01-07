using System;

const long Divider = 20201227;
const long PublicKeySubjectNumber = 7;
const long CardPublicKey = 15733400, DoorPublicKey = 6408062;

long LoopSizeForPublicKey(long publicKey)
{
    long result = 1;

    for (long i = 1; ; i++)
    {
        result = (result * PublicKeySubjectNumber) % Divider;
        if (result == publicKey) return i;
    }
}

long GetEncryptionKey(long subjectNumber, long loopSize)
{
    long result = 1;
    
    for (long i = 0; i < loopSize; i++)
        result = (result * subjectNumber) % Divider;

    return result;
}

long cardLoopSize = LoopSizeForPublicKey(CardPublicKey);
Console.WriteLine(GetEncryptionKey(DoorPublicKey, cardLoopSize));
