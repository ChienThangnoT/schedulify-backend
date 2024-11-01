using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Enums
{
    public enum ETimetableCreator
    {
        CrossoverUsingClassChromosome,
        CrossoverUsingTeacherChromosome,
        EnhanceUsingTabuSearch
    }
    public enum ESelectionMethod
    {
        RankSelection, RouletteWheel, TournamentSelection, MultiObjective
    }
    public enum ECrossoverMethod
    {
        SinglePoint, MultiPoint, Uniform
    }
    public enum EChromosomeType
    {
        ClassChromosome, TeacherChromosome
    }
    public enum EReplacementStrategy
    {
        FullReplacement, PartialReplacement
    }
    public enum EMutationType
    {
        Default, AdaptiveMutation, DiverseMutations
    }

    public class TimetableCreatorParameters
    {
        public readonly int PopulationSize = 100;
        public readonly int NumberOfGenerations = 1000;
        public readonly float CrossoverRate = 0.5f;
        public readonly float MutationRate = 0.1f;
        public readonly ESelectionMethod SelectionMethod = ESelectionMethod.RankSelection;
        public readonly ECrossoverMethod CrossoverMethod = ECrossoverMethod.SinglePoint;
        public readonly EChromosomeType ChromosomeType = EChromosomeType.ClassChromosome;
        public readonly EMutationType MutationType = EMutationType.Default;
    }
}
