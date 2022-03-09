using System.Collections;
using DG.Tweening;

namespace Interfaces
{
    public interface IFadeUI
    {
        public IEnumerator ShowAndStayPanel();
        public IEnumerator ShowHidePanel();
        public void FadeIn(float duration);
        public void FadeOut(float duration);
       
    }
}