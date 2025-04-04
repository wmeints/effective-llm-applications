﻿namespace Chapter7.ValidationDatasetGeneration.Shared;

public class LanguageModelConfiguration
{
    public required string ChatCompletionModel { get; set; }
    public required string TextEmbeddingModel { get; set; }
    public required string Endpoint { get; set; }
    public required string ApiKey { get; set; }
}