{#prompt-testing-and-monitoring}
# Testing and Monitoring Prompts

LLM-based applications are harder to test than regular applications because we have an
layer of uncertainty in the application. The LLM will provide a different response
each time you call it. In this chapter we'll learn how to test and monitor prompts
while dealing with the uncertainty of using an LLM.

Of all the things you can do to improve the quality of your LLM-based application, I think testing and monitoring
are one of the most important. Gathering enough test data will take time, but it's worth it.

We'll cover the following topics:

- Testing prompts with deterministic and model-based testing methods
- Monitoring prompt interactions in production

Let's start by discussing how to test prompts.

## Using deterministic testing methods to validate prompts

- Introduce property-based tests as a solution to validate prompts.
- Explain why you would use a property-based approach to testing prompts.

### Checking rendered prompts for correctness

- Explain how you can check the rendered prompts for correctness.
- Show an example of a unit-test with prompt rendering and validation.

### Using property-based testing to verify responses

- Show how you can analyze a prompt response and test for particular properties.
- Explain the benefits and limitations of property-based testing of prompts.

## Using model-based testing methods to validate prompts

- Introduce model-based testing and how it differs from the deterministic methods.
- Explain that we'll only look at G-Eval, but there are other model-based approaches available.
- This field is changing quickly just like LLMs, so it's important to keep an eye out for new developments.

### Understanding the general approach to model-based testing

- Explain the benefits and limitations of model-based testing approaches.
- Explain what you can and can't assert in a model-based test.
- Explain what sort of data you need to build model-based tests.

### Using G-Eval to evaluate prompts and their responses

- Explain how G-Eval helps you evaluate prompts and their responses.
- Show an example unit-test setup with xunit and G-Eval based metric evaluation.
- Repeat the limitations of G-Eval to make sure people know what they're getting into.

## Monitoring prompt interactions in production

From the previous section we learned how to test prompts in a controlled environment.
Testing only gets you so far sadly. The samples we collected in the previous section are
limited to what you can come up with and might not be representative of what users are
going to do. The only way we can get test data that's representative of the real world
is to collect it from production.

Let's look at using monitoring to keep an eye on how the application is being used in
production and using collected telemetry to improve the application.

### Writing monitoring data to application insights

- Enabling opentelemetry with semantic kernel
- What data is written to the opentelemetry sink
- Configuring a dashboard for your LLM-based application

### Extracting test data from application insights

- Use application insights to collect telemetry data on prompt interactions.
- Precautions you need to take when extracting test data from production and integrating it into your test set.
- How to handle privacy issues when extracting test data from production.

## Summary

## Further reading

- [GPTScore: Evaluate as you desire](https://arxiv.org/abs/2302.04166)
- [G-Eval: NLG Evaluation using GPT-4 with Better Human Alignment](https://arxiv.org/abs/2303.16634)
