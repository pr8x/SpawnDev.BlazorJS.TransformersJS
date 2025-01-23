using System.ComponentModel.DataAnnotations;

namespace SpawnDev.BlazorJS.TransformersJS
{
    public class GenerateSpeechOptions
    {
        [Required]
        public SpeechT5HifiGan Vocoder { get; set; }
    }
}
