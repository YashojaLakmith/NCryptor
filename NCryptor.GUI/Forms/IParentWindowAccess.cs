namespace NCryptor.GUI.Forms
{
    /// <summary>
    /// Defines methods for a form to pass its visibility control to its children.
    /// </summary>
    internal interface IParentWindowAccess
    {
        void HideParentWindow();
        void ShowParentWindow();
    }
}
