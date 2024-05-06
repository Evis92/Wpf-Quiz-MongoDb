using System.Net.Http.Headers;


namespace Common.DTOs;

public record QuizRecord(string Id, string QuizName, string Description, List<string> Questions);