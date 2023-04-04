﻿// See https://aka.ms/new-console-template for more information
using OpenAI;
using OpenAI.Chat;

Console.WriteLine("Hello, World!");

var auth = OpenAIAuthentication.LoadFromDirectory();

var openAIClient = new OpenAIClient(auth);

//var

var files = Directory.GetFiles(@"data\", "*.jsonl");
//foreach (var file in files)
{
    //var fileData = await openAIClient
    //                        .FilesEndpoint
    //                        .UploadFileAsync(file, "fine-tune");
    //var fineTuneJobRequest = new CreateFineTuneJobRequest(fileData, model: "davinci");
    //var fineTuneJob = await openAIClient
    //                                .FineTuningEndpoint
    //                                .CreateFineTuneJobAsync(fineTuneJobRequest);
    var customFineTunedModel = string.Empty;

    //customFineTunedModel = fineTuneJob.FineTunedModel;

    var fineTuneJobs = await openAIClient
                                    .FineTuningEndpoint
                                    .ListFineTuneJobsAsync();

    var input = string.Empty;
    Console.WriteLine("press any key to RetrieveFineTuneJobInfoAsync, press q exit ...");
    while ("q" != (input = Console.ReadLine()))
    {
        foreach (var job in fineTuneJobs)
        {
            Console.WriteLine($"{job.Id} -> {job.CreatedAt} | {job.Status}");
            var j = await openAIClient.FineTuningEndpoint.RetrieveFineTuneJobInfoAsync(job);
            Console.WriteLine($"{j.Id} -> {j.Status}");
            var fineTuneEvents = await openAIClient.FineTuningEndpoint.ListFineTuneEventsAsync(j);
            Console.WriteLine($"{j.Id} -> status: {j.Status} | event count: {fineTuneEvents.Count}");
            foreach (var @event in fineTuneEvents)
            {
                Console.WriteLine($"\t{@event.CreatedAt} [{@event.Level}] {@event.Message}");
            }
            customFineTunedModel ??= j?.FineTunedModel;
        }
    }

    Console.WriteLine("press any key to Test Prompt, press q exit ...");
    while ("q" != (input = Console.ReadLine()))
    {
        input = "who is best?";
        var chatPrompts = new List<ChatPrompt>
            {
                  new ChatPrompt("system"     , input!)
                , new ChatPrompt("user"       , input!)
                , new ChatPrompt("assistant"  , input!)
                , new ChatPrompt("user"       , input!)
            };
        var chatRequest = new ChatRequest(chatPrompts, customFineTunedModel);
        var response = await openAIClient.ChatEndpoint.GetCompletionAsync(chatRequest);
        Console.WriteLine(response.FirstChoice!);
    }


    //var result = await openAIClient.FilesEndpoint.DeleteFileAsync(fileData);

}

//File.Delete(localTrainingDataPath);