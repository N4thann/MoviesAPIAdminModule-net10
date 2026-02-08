namespace Domain.SmartEnums
{
    public class AwardCategory : Enumeration
    {
        // Major Categories
        public static readonly AwardCategory BestPicture = new(1, "Best Picture");
        public static readonly AwardCategory BestDirector = new(2, "Best Director");
        public static readonly AwardCategory BestActor = new(3, "Best Actor");
        public static readonly AwardCategory BestActress = new(4, "Best Actress");
        public static readonly AwardCategory BestSupportingActor = new(5, "Best Supporting Actor");
        public static readonly AwardCategory BestSupportingActress = new(6, "Best Supporting Actress");
        public static readonly AwardCategory BestOriginalScreenplay = new(7, "Best Original Screenplay");
        public static readonly AwardCategory BestAdaptedScreenplay = new(8, "Best Adapted Screenplay");

        // Technical Categories
        public static readonly AwardCategory BestCinematography = new(20, "Best Cinematography");
        public static readonly AwardCategory BestFilmEditing = new(21, "Best Film Editing");
        public static readonly AwardCategory BestProductionDesign = new(22, "Best Production Design");
        public static readonly AwardCategory BestCostumeDesign = new(23, "Best Costume Design");
        public static readonly AwardCategory BestMakeupAndHairstyling = new(24, "Best Makeup and Hairstyling");
        public static readonly AwardCategory BestSound = new(25, "Best Sound");
        public static readonly AwardCategory BestVisualEffects = new(26, "Best Visual Effects");

        //  Music Categories
        public static readonly AwardCategory BestOriginalScore = new(30, "Best Original Score");
        public static readonly AwardCategory BestOriginalSong = new(31, "Best Original Song");

        // Feature Film Categories
        public static readonly AwardCategory BestAnimatedFeature = new(40, "Best Animated Feature Film");
        public static readonly AwardCategory BestInternationalFeature = new(41, "Best International Feature Film");
        public static readonly AwardCategory BestDocumentaryFeature = new(42, "Best Documentary Feature");

        // Short Film Categories
        public static readonly AwardCategory BestAnimatedShort = new(45, "Best Animated Short Film");
        public static readonly AwardCategory BestLiveActionShort = new(46, "Best Live Action Short Film");
        public static readonly AwardCategory BestDocumentaryShort = new(47, "Best Documentary Short Subject");

        // Festival-Specific and Other Award Categories
        public static readonly AwardCategory PalmeDor = new(100, "Palme d'Or");
        public static readonly AwardCategory GoldenLion = new(101, "Golden Lion");
        public static readonly AwardCategory GoldenBear = new(102, "Golden Bear");
        public static readonly AwardCategory GrandJuryPrize = new(103, "Grand Jury Prize");
        public static readonly AwardCategory BestEnsemble = new(104, "Best Ensemble Cast");

        /// <summary>
        /// Initializes a new instance of the <see cref="AwardCategory"/> class.
        /// The constructor is private to enforce the type-safe enum pattern,
        /// ensuring that only the predefined static members can be instantiated.
        /// </summary>
        /// <param name="id">The unique integer identifier for the category.</param>
        /// <param name="name">The display name of the category.</param>
        private AwardCategory(int id, string name) : base(id, name) { }
    }
}