namespace Brimborium.TextGenerator;

public enum TGSourceInformationMode { NoInfo }

public struct TGSourceInformation
{
    public readonly TGSourceInformationMode Mode;

    public TGSourceInformation()
    {
        this.Mode = TGSourceInformationMode.NoInfo;    
    }

    public TGSourceInformation(TGSourceInformationMode mode)
    {
        this.Mode = mode;
    }
}