using DailyGlowApi.Models;

namespace DailyGlowApi.Dtos;

// Two products in a routine that should not be used together
public record ClashDto(
    string FirstProduct,
    string SecondProduct,
    ActiveIngredient FirstIngredient,
    ActiveIngredient SecondIngredient,
    string Advice);

// The answer of the clash check for one routine
public record ClashReportDto(
    int RoutineId,
    string RoutineName,
    bool HasClashes,
    IReadOnlyList<ClashDto> Clashes);
