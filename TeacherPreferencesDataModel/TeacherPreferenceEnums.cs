﻿using System.ComponentModel;

namespace TeacherPreferencesDataModel
{
    public enum Priority
    {
        Mandatory,
        Desirable,
        Neutral,
        NotDesirable,
        Avoidable
    }

    public enum DayOfWeek
    {
        AllWeek,
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday
    }

    public enum AudienceEquipmentType
    {
        Projector,
        WhiteBoard,
        NumberOfBoards,
        ComputerAudience,
        Computer,
        TimeService,
        Capacity,
        Blackboard
    }

    public enum Department
    {
        None,
        Astronomy,
        Astrophysics,
        Algebra,
        Geometry,
        ComputationalMathematics,
        DifferentialEquations,
        Informatics,
        InformationAndAnalyticalSystems,
        Hydroaeromechanics,
        OperationsResearch,
        MathematicalAnalysis,
        MathematicalPhysics,
        CelestialMechanics,
        ParallelAlgorithms,
        AppliedCybernetics,
        SystemProgramming,
        StatisticalModeling,
        TheoreticalAndAppliedMechanics,
        TheoreticalCybernetics,
        ProbabilityTheoryAndMathematicalStatistics,
        PhysicalMechanics,
        ForeignLanguages,
        TheoryOfElasticity
    }

    public enum ClassType
    {
        Lecture,
        Laboratory,
        Seminar,
        Practical,
        None
    }

    public enum GeneralPreferenceType
    {
        MaxDaysPerWeek,
        MinDaysPerWeek,
        FreeDaysPerWeek,
        MaxClassesPerDay,
        MinClassesPerDay,
        Compactness,
        IntersectTimeWithTeacher,
        Gaps
    }

    public enum SpecificPreferenceType
    {
        UniteGroups,
        UniteClasses,
        AlternateBySubgroups,
        UniteSubgroups,
        OneClassInTwoWeeks,
        InOneDay
    }
}