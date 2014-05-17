namespace Tuto.Services.Assembler
{
    class AvsFadeOut : AvsFadeIn
    {
        protected override string Format { get { return "{0} = FadeOutTime({1}, {2})"; } }
    }
}