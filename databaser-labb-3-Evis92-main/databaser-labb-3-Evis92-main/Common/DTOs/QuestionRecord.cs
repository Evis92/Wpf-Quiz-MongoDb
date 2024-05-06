namespace Common.DTOs;

public record QuestionRecord(string Id, string QuestionTxt, string Category, List<string> AnswerOptions, string CorrectAnswer);