namespace Assets.Scripts.Interfaces
{
    public interface ICustomSelectable
    {
        void Select(bool keyboard = false);
        void Deselect();
        void Press();
        void Release();
        void Click();
    }
}
