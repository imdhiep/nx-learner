using NXOpen;

namespace PartList
{
    #region File: Unload.cs
    public class Unload
    {
        public static int GetUnloadOption(string dummy)
        {
            return (int)NXOpen.Session.LibraryUnloadOption.Immediately;
        }
    }
    #endregion
}

