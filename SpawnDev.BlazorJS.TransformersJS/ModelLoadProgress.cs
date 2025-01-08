namespace SpawnDev.BlazorJS.TransformersJS
{
    public class ModelLoadProgress
    {
        /// <summary>
        /// The model file being loaded
        /// </summary>
        public string File { get; set; }
        /// <summary>
        /// Progress status
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// The model name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The number of bytes loaded
        /// </summary>
        public long? Loaded { get; set; }
        /// <summary>
        /// The progress percentage from 0 to 100
        /// </summary>
        public float? Progress { get; set; }
        /// <summary>
        /// The total number of bytes to load
        /// </summary>
        public long? Total { get; set; }
    }
}
