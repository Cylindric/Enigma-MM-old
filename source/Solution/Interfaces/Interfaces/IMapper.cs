namespace EnigmaMM.Interfaces
{
    /// <summary>
    /// Provides a simple interface for passing Mappers around.
    /// </summary>
    public interface IMapper
    {
        /// <summary>
        /// Renders the default map(s) for this renderer.
        /// </summary>
        void Render();

        /// <summary>
        /// Renders the specified map(s) for this renderer based on mapper-specific criteria.
        /// </summary>
        /// <param name="type">type of map to render</param>
        void Render(string type);
    }
}
