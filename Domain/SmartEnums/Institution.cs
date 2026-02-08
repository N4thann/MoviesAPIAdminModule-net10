namespace Domain.SmartEnums
{
    public class Institution : Enumeration
    {
        public static readonly Institution AcademyAwards = new(1, "Academy of Motion Picture Arts and Sciences (Academy Awards)");
        public static readonly Institution GoldenGlobes = new(2, "Hollywood Foreign Press Association (Golden Globes)");
        public static readonly Institution BAFTA = new(3, "British Academy of Film and Television Arts (BAFTA)");
        public static readonly Institution CriticsChoice = new(4, "Critics Choice Association");
        public static readonly Institution IndependentSpirit = new(5, "Film Independent (Independent Spirit Awards)");
        public static readonly Institution Satellite = new(6, "International Press Academy (Satellite Award)");
        public static readonly Institution SundanceFestival = new(7, "Sundance Film Festival");
        public static readonly Institution CannesFestival = new(10, "Cannes Film Festival");
        public static readonly Institution VeniceFestival = new(11, "Venice International Film Festival");
        public static readonly Institution BerlinFestival = new(12, "Berlin International Film Festival");
        public static readonly Institution TorontoFestival = new(14, "Toronto International Film Festival (TIFF)");
        public static readonly Institution LocarnoFestival = new(15, "Locarno International Film Festival");

        public static readonly Institution DGA = new(30, "Directors Guild of America (DGA)");
        public static readonly Institution PGA = new(31, "Producers Guild of America (PGA)");
        public static readonly Institution WGA = new(32, "Writers Guild of America (WGA)");
        public static readonly Institution SAG = new(33, "Screen Actors Guild (SAG Awards)");
        public static readonly Institution ACE = new(34, "American Cinema Editors (Eddie Awards)");

        public static readonly Institution Saturn = new(40, "Academy of Science Fiction, Fantasy and Horror Films (Saturn Awards)");
        public static readonly Institution MTVMovieAwards = new(41, "MTV Movie & TV Awards");
        public static readonly Institution GoldenRaspberry = new(42, "Golden Raspberry Awards (Framboesa de Ouro)");
        public static readonly Institution YoungArtist = new(43, "Young Artist Awards");

        public static readonly Institution NYFCC = new(50, "New York Film Critics Circle");
        public static readonly Institution LAFCA = new(51, "Los Angeles Film Critics Association");
        public static readonly Institution CFCA = new(52, "Chicago Film Critics Association (CFCA Awards)");
        public static readonly Institution LVFCS = new(53, "Las Vegas Film Critics Society (Sierra Award)");
        public static readonly Institution PFCS = new(54, "Phoenix Film Critics Society (PFCS Award)");
        public static readonly Institution BlackReel = new(55, "Black Reel Awards");

        public static readonly Institution HollywoodFilm = new(60, "Hollywood Film Award");
        public static readonly Institution Image = new(61, "NAACP Image Award");

        /// <summary>
        /// The primary constructor for an institution.
        /// </summary>
        /// <param name="id">The unique identifier for the institution.</param>
        /// <param name="name">The name of the institution.</param>
        private Institution(int id, string name) : base(id, name) { }
    }
}
