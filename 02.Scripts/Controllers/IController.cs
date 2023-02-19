namespace HTH.InputHandlers
{
    public interface IController
    {        
        bool controllable { get; set; }
        bool RequestControl();
        bool RequestReturn();
    }
}
