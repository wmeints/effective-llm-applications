{#essential-llmops-knowledge} 
# Essential LLMOps knowledge

In [#s](#understanding-llms), we explored the basic concepts and terminology around LLMS
and briefly touched on the essentials for building and operating LLMs. In this chapter,
we'll examine the essentials of operating LLMs in more detail before we dive into
working with Semantic Kernel. Consider this chapter the moment when we start planning
the LLM-based applications we're building in the upcoming chapters.

I thought of showing your Semantic Kernel before talking about operating LLMs. However,
the knowledge in this chapter is essential to learn first because it's easy to get lost
in the details of implementing various patterns with an LLM and lose sight of the bigger
picture. You must plan some things out for yourself before you start building. Learning
how to send a prompt to an LLM is one thing, but knowing how to manage the lifecycle of
an LLM-based application is another. And you need both to be successful.

This chapter will help you understand the basic operations of building and hosting
LLM-based applications in greater detail.

We'll cover the following topics:

- What is LLMOps?
- Testing LLM-based applications
- Monitoring and evaluation of your application
- Cost management and optimization
- Rate limiting and capacity planning
- Performance considerations
- Failover strategies
- Security and privacy concerns
- Considerations when looking for tools

Let's first cover what LLMOps is compared to the other types of Ops approaches out
there.

## What is LLMOps?

Most of you will already know what DevOps is; it combines development and operations
practices to optimize the process of building, testing, and deploying software. Often,
the term DevOps is mixed with Continuous Delivery. DevOps has become so popular that
people started using the term MLOps to refer to similar practices for managing the
lifecycle of machine learning projects. Now that we have LLMs, people have begun to use
the term LLMOps to refer to the practices involved in managing LLM-based projects.

When I discuss LLMOps, I like to take a layered approach, as shown in
[#s](#llmops-layering).

{#llmops-layering} 
![Layering DevOps, MLOps, and LLMOps](figure-1-llmops.png)

LLM-Based projects are software-based projects and should follow DevOps practices. Most
of the time, the LLM only makes up a small part of the project, so most of your time
will go into managing the DevOps aspects of your project.

Since LLMs are machine-learning models, applying MLOps practices to a project that uses
an LLM is useful. So, let's explore what that means. MLOps is about managing the
lifecycle of machine learning models. It's about managing the data, the model, the
configuration, and the code to produce and use the model. When you apply MLOps, you
typically:

- Track experiments to train, test, and evaluate the model.
- Monitor the model in production to measure how circumstances change so you can retrain
the model to improve it over time.
- Manage the model configuration to ensure it's always up-to-date and
running in the right environment.

These concepts apply to LLMs, too. Except that, it's extremely unlikely you'll train
your LLM. You may fine-tune one if you want to go very advanced, but I doubt most of you
will ever do that. So, LLMOps focuses more on managing the interaction between the LLM
and the rest of your application.

On the internet, you'll find many definitions of LLMOps and even dedicated tools to help
you manage LLMs. But I like to stick to the basics, as most LLMOps tools aren't very
mature yet.

I've found that the following seven aspects deserve special attention when building and
operating LLM-based applications:

1. Testing LLM-based applications
2. Monitoring and evaluation
3. Cost management and optimization
4. Rate limiting and capacity planning
5. LLM performance and the user experience of your application
6. Failover strategies
7. Security and privacy in an LLM-based application

Let's dive into each of these aspects in more detail.

## Testing LLM-based applications

Testing LLM-based applications is different from testing regular software applications.
In regular software applications, you can use tests that assert a true or false
statement. Typically, you'll write a combination of unit tests, integration tests, and
possibly even acceptance tests to validate that your application is working as expected,
using a boolean approach to verify that some rules hold for your system.

You need to approach testing LLM-based applications differently from regular software.
It isn't as black and white. LLMs give you a different response every time because of
what we discussed in [#s](#understanding-llms); they use output sampling techniques. You
need a different testing approach to validate that your LLM-based application is
working.

Where unit and integration tests should already be standard practice, you must apply one
extra layer of tests for LLM-based applications. LLM-based applications need
non-deterministic tests. This sounds weird because why would you want a test for which
you aren't getting a yes or no? It will be hard to avoid [flaky tests][FLAKY_TESTS].

The reason is that you're not looking for an absolute answer; you won't get it. Instead,
you want the correct answer most of the time. We're trying to lower our solution's risk
as much as possible. We need to accept that, just like regular software, the system
sometimes fails for unknown reasons. But we have a good idea of where the problem is
coming from for an LLM-based application.

One testing strategy that works great in an LLM-based is running multiple samples
through the application and then having the test cast a majority vote to determine if
the call to the LLM produces the correct output.

Another great strategy is to use metrics with a value range to determine that the output
is within acceptable limits for your chosen metric.

I can imagine that knowing just these rules for testing LLM-based applications generates
more questions than answers. For now, it's essential to understand that you need to
change your perspective on testing. Throughout the remaining chapters of this book, I'll
show you how to test LLM-based applications in more detail:

- In [#s](#the-art-and-nonsense-of-prompt-engineering), we'll look at how to use prompt
testing as a form of unit testing for your prompts.
- In [#s](#prompt-testing-and-monitoring), we'll use integration testing to validate that the LLM can make tool
calls by applying integration tests.
- In [#s](#enhancing-llms-with-tools), we dive into integration tests for a search and an LLM combination.

{#llmops-monitoring}
## Monitoring and evaluation of your application

You can consider testing as a way to ensure you have a safety net to catch problems
before they happen in production. Monitoring is the second safety net you need to catch
problems while running in production.

There are two essential aspects to monitoring LLM solutions. Unsurprisingly, you must
monitor for any infrastructure and technical problems with your code. This is more of a
DevOps practice than an LLMOps practice.

The LLMOps approach to monitoring focuses on the interaction between your application
and the LLM. Machine-learning models like LLMs tend to fail silently in production.
People change their interaction patterns as the model is used, and the model starts to
generate less accurate responses. This shift is called model drift or concept drift.

When applying patterns like Retrieval Augmented Generation (RAG), tracking interactions
between the LLM and external data sources is essential. As information changes in your
data sources, the LLM may generate less accurate responses, which you could call data
drift.

We'll cover monitoring in greater detail as we go through the remaining chapters in this book:

- In [#s](#the-art-and-nonsense-of-prompt-engineering), we use monitoring to collect data needed to iterate on your prompts.
- In [#s](#prompt-testing-and-monitoring), we cover how to monitor calls to tools connected to the LLM.
- In [#s](#enhancing-llms-with-tools), we'll look at how to monitor the interaction between the LLM and a search engine.

Monitoring interactions is essential to keeping your LLM-based application running smoothly in production. When setting up monitoring, I recommend including monitoring costs as well.

## Cost management and optimization

Cost management is a big part of LLMOps. When you host an LLM-based application in the
cloud, you can quickly run into high costs if you build just the right kind of logic so
that a workflow never ends or becomes very inefficient because the LLM generates weird
output that causes the workflow to make the wrong decision.

Let's imagine you built an LLM-based application that can decide if a text is of enough
quality to be used by the user. If not, it can improve the text based on feedback
generated using the LLM. This sounds like a great idea, which we'll cover in Chapter 11
but has a significant downside. When will the loop end? If it's good enough? Well, what
if the LLM generates output asking for more changes? You can't just let the workflow run
forever because it will cost you a lot of money. Consider limiting the cost of running
an LLM-based application.

The answer lies in two things: Limit how much money you're willing to burn on the loop
in your application and set a hard limit on how many times the agent is allowed to try
to improve the text.

In [#s](#understanding-llms), we covered various LLM providers from which to choose.
Each has its cost management solution, making it hard to give you a general tutorial on
managing costs. Instead, let me point you towards the documentation of the provider
you're using. They will have a section on cost management:

- [OpenAI](https://platform.openai.com/usage) lets you set a monthly limit for your
organization. This is a hard limit that is reset every month.
- [Microsoft
Azure](https://learn.microsoft.com/en-us/azure/cost-management-billing/manage/spending-limit)
Let's you set a spending limit for your subscription. Additionally, you can set a token
per minute limit for the LLM to slow things down.
- [Google](https://cloud.google.com/billing/docs/how-to/budgets) doesn't have hard
spending limits. Instead, you need to spend more money on a budget alerting solution
that invokes the billing API to stop the billing for your project.
- [Anthropic](https://docs.anthropic.com/en/api/rate-limits) lets you set your
  organization's rate and spending limits. This limit is reset every month.

Generally, I recommend setting a reasonable spending limit for your project. Start on
the lower end so you don't let things get out of hand too early. Run a test with a small
group to establish a cost benchmark and then adjust the spending and rate limits
accordingly. This way, you can keep the costs under control.

Now that we have the essential monitoring concepts let's deal with failure modes in your
LLM-based application.

{#llmops-rate-limits} 
## Dealing with rate limits and capacity planning

All LLM providers are dealing with an enormous influx of people who want to use their
services. They can't get the GPU power they need to fulfill all the requests. You're
going to have to deal with that.

The LLM providers introduced rate limits to control the traffic flow into their data
centers, and I've found that you will hit those limits very quickly.

For some providers like Azure, you can assign a quota to your LLM instance. This quota
is a token budget your application can spend per minute to call the LLM.

It's important to note here that the quota is not a hard limit or a guarantee for how
fast you'll get a response. An LLM provider makes [an estimation][TOKEN_ESTIMATION] of
how many tokens you need for a request. It then processes the request if it fits in the
budget.

If you run out of tokens, you'll have to wait a bit before you can make another call.
The best way to work with rate limits is to have a retry mechanism with an exponential
backoff. If you do this correctly, it's another layer of infrastructure code that
doesn't get in the way of you building your application.

Usually, you'll find that you get higher speeds when you set a higher quota. But it's
best to measure how many tokens you typically need in your application and set the value
for the quota to a reasonable value. You can always increase the quota if you need more.

When I can, I'll examine monitoring data during early testing to determine how many
tokens per minute we need and set the quota accordingly. You need to strike a balance
between costs and user experience here because rate limits help you control costs, too.
The lower the quota, the lower the amount of tokens that can be spent per minute, and
the lower the bill. However, too many retries can lead to a bad user experience.

In [#s](#the-art-and-nonsense-of-prompt-engineering), we'll cover configuring parameters
for your prompt to maximize your quota.

## LLM performance and the user experience of your application

I mentioned performance in the previous section, and it's a thing for LLM-based
applications. LLMs are very slow, and you need to think about how to work around that
limitation because you can't speed them up yourself. You can increase rate limits to
speed things up a little, but that's about it.

Let me shift your perspective about performance in software applications. Under normal
circumstances, you need to look for performance bottlenecks in all areas of your
application. And it's likely you can solve many of those bottlenecks yourself. When
using an LLM in your application, it is important to know that the LLM will be your
biggest bottleneck, and you can't speed it up by much.

There are two things that you need to think about when optimizing LLM-based application
performance:

- First, you must think about ways to improve the user experience. For example, you can
let the LLM stream the response to a prompt. This way, the user receives an answer in
chunks but quickly.
- Second, you need to think less about the performance of the code around the call to an
LLM. It won't matter as much what you do there because the LLM is the slowest piece in
the puzzle.

Do keep in mind that you'll want to optimize the rest of the code that doesn't call the
LLM to be fast; otherwise, you're just adding to the problem. I've found that if the
rest of the application is nice and fast, users are more forgiving about the LLM being
slow.

{#llomops-failover-strategies} 
## Failover strategies

As if performance wasn't hard enough, we got another nice present from the LLM
providers. LLM providers are sometimes unavailable because you've hit a hard limit or
because they're overloaded. Anthropic had this problem frequently when I wrote this
chapter, where you receive overloaded errors and can only call the LLM again after a few
hours. This can be problematic when you run a production application that needs some
form of availability.

In many cases, I've found that letting the user know that the LLM is unavailable is
enough if you have no availability requirements. Many of my clients don't have any
availability requirements because they're just starting out and know that they can't
guarantee 100% uptime.

From experience, I learned that despite the lack of availability requirements, it's
beneficial to have a failover strategy in place. For example, you could have GPT-4o as a
fallback for Claude Sonnet in your application. This way, you can switch to GPT-4o when
Claude Sonnet is unavailable.

Regardless of what your client thinks, it's good to spend some time planning for
failover scenarios, as this significantly impacts the user experience and the
development effort you need to spend.

{#llmops-security} 
## Security and privacy in an LLM-based application

We've covered quite a few LLMOps essentials. You may have noticed that not many of these
aspects are well understood by users of LLM-based applications. This is especially true
for security and privacy concerns.

I've not had many security discussions with clients about LLMs. This is because we're
just getting started using LLMs in production. Not many people realize how dangerous
LLMs are. I could write an entire book about this topic, but I'll have to keep this
shorter because we need to talk about other things. But let me give you the basics so
you know what you're getting yourself into.

There are three main topics when we talk about security in LLMs:

- **Data privacy:** Customers and users will talk about data privacy because they're
worried about the provider stealing their data to train the LLM.
- **Application security:** This is about protecting your application from being hacked.
This is a big topic because LLMs are very powerful and can be used to generate malicious
content.
- **User safety:** This is about protecting your users from being exposed to harmful
content generated by the LLM. LLMs can generate harmful content or misleading content
that can be used to manipulate people.

Let me go over each of these topics to give you a direction on how to think about them.

{#llmops-data-privacy} 
### Data privacy

Before you start using an LLM, you need to think about the data you're sending to it.

The [Samsung incident][SAMSUNG_INCIDENT] teaches us an important lesson: If your data is
going to be used for training, it is likely to end up on someone else's desk at some
point in the future. Hackers are actively probing with prompts to find out what data is
being used to train the LLM.

Sometimes, it's better not to use an LLM if you're worried about company secrets being
leaked. For most cases, however, it's good enough to agree with the LLM provider and ask
it not to use your data for training. All LLM Providers offer this option today and have
a section about it in their legal agreements.

It is essential to understand that security is a trade-off. You can't have 100% security
because then you wouldn't use LLMs in the first place and miss out on their value.
However, you can't wholly live without security, either. Have an active discussion with
your client about the data you're sending to the LLM and the risks involved, and then
decide based on that.

There's another closely related topic to data privacy that I want to address here. In
increasingly increasing countries, some laws require you to be careful with personal
identifiable information (PII). For example, in Europe, you need to comply with GDPR. In
California, you need to comply with CCPA. These laws require you to be careful with the
data you're processing.

Managing personal data is a topic you should address alongside the other security
requirements when planning an LLM-based application.

Many cloud providers offer standard tools to filter out PII from text. For example,
Azure has a tool called [Text Analytics][PII_FILTER] that can filter out PII from text.
This way, you can avoid sending PII to the LLM or storing it in your application.
Consider it an extra safety measure on top of ensuring people know they're not supposed
to send in PII.

{#llmops-application-security} 
### Application Security

Securing modern cloud-native applications is challenging. Adding an LLM to the mix opens
up a new chapter in application security. And it's a relatively new chapter, so there's
not much information on securing an LLM-based application. I will cover some LLM
security strategies around security in greater detail in the following chapters, but I
want to make sure you have a good overview of the topic first.

Applying a deep defense strategy is the best way to handle application security. You'll
want to use multiple layers of defenses to ensure that if a hacker gets through one
layer, it will have to work hard to get through the next. A layered defense also helps
keep an attack's blast radius to a minimum.

One of the best moves you can make in terms of application security is not giving your
application access to resources it shouldn't have access to. I see many people using
Copilot 365 and oversharing information because of bad defaults and the admin not being
aware that you should be careful. Microsoft even made a
[blueprint][OVERSHARING_BLUEPRINT] to help you fix the issues.

If there's one thing you should learn from this, it should be this: Make deliberate
choices in what you hand to an LLM. It's better to start with nothing.

If your application needs a database, don't give it admin access. It does sound like old
advice, but it is extra relevant now that we can let the LLM generate SQL queries and
other nasties. You don't want to allow the LLM to drop tables in your database. I don't
recommend letting the LLM generate SQL queries and then run those queries.

If you're interested in learning more about application security, I highly recommend
looking at the following two resources:

- [OWASP Top 10 for LLM applications][OWASP_LLM]
  - Covers the top 10 most occurring security risks in LLM-based applications.
- [MITRE Atlas][MITRE_ATLAS] - Covers all possible attacks against AI systems

Also, you should adopt a security-first mindset when developing LLM-based applications.
You can do this with just a few steps. I wrote a blog post about this topic that you can
read [here][MANAGING_LLM_THREATS].

While application security is a large portion of the security concerns you'll have to
deal with, there's another equally important part — user safety.

{#llmops-user-safety} 
### User safety

We've all seen online videos and articles showing LLMs generating harmful or silly
content. If you ask me, it's a genuine concern because we attribute this to the LLM's
limitations, while the developers do not consider the consequences of their
application's output.

We often call an LLM's weird output a hallucination, but that's not really what's
happening. Let's take a step back to understand how an LLM works to fully understand
what I mean by this statement.

When you ask an LLM to generate output for a prompt, the LLM predicts the most likely
text that follows your prompt, as we covered in [#s](#understanding-llms). The LLM uses
an attention mechanism combined with an embedding layer.

The embedding layer gives meaning to the most relevant context surrounding a token in
the context that we're looking at. The attention mechanism is trained to focus on the
most relevant parts of the context we're working with.

The embeddings and the attention mechanism are trained on all sorts of data, including
harmful content. It is impossible to filter out the harmful training content because
we're talking about trillions of tokens used to train LLMs. This means that the LLM can
generate harmful content because it has seen it before, and it's essentially parroting
the content it's seen before. It's just plain math.

As a developer of an LLM-based application, you need to be aware of this and take other
measures to keep your users safe. You can't fix the model, so you must fix something
else. To protect your user against malicious content, you must apply output sanitation.

Luckily, there are tools to detect malicious content and filter it out. For example,
Azure has a tool called [AI Content Safety][AZ_CONTENT_SAFETY]. Google has the same
feature called [text moderation](GA_CONTENT_SAFETY). Finally, AWS also has a solution
called [content moderation][AWS_CONTENT_SAFETY]. So, it's not a lot of work to integrate
content filtering even when you're unsure if your application will generate harmful
content.

## Considerations when looking for tools

When you browse the internet regularly and look at social media like LinkedIn, X, and
BlueSky, you'll see many ads about LLMOps tools. I personally don't use any of these
tools right now because their value isn't clear to me yet. There are existing tools for
many of the aspects mentioned in this chapter, and I recommend reading the rest of this
book before you start browsing for an LLMOps solution. You'll have a better
understanding of what you need and what you don't need.

I do want to give you a few pointers on what to look for when you're looking for an
LLMOps tool:

- As tools are evolving quickly, you'll likely switch one or two times to a newer tool
  the following year. So, make sure you choose a tool that you can easily migrate away
  from.

- Make sure that the tool has the proper measures in place to protect your data.
  Exposing your data through the LLMOps provider while worrying about the LLM itself is
  silly. Make sure the tool has a good security track record.

- Make sure the tool has a good support team. You'll likely run into issues with the
  tool because it's new and inexperienced. You'll need a good support team to help you
  out.

For most teams I work with, integrating the LLM aspects into the rest of their DevOps
practices is essential to keeping their mental model simple. I recommend you do the same
until a clearer picture of LLMOps tools emerges.

## Summary

In this chapter, we covered the essentials of LLMOps, the differences between regular
software projects and LLM-based projects, the importance of testing LLM-based
applications, and the need to monitor and evaluate them. We also covered cost management
and optimization, rate limiting and capacity planning, performance considerations,
failover strategies, security and privacy concerns, and considerations when looking for
tools.

In the next chapter, we'll explore Semantic Kernel, a tool I've been using to build
LLM-based applications. We'll learn what it looks like and how to set up a project with
it, and then we can start learning about LLM-based workflows and agents in later
chapters.

## Further Reading

- [An introduction to LLMOps](https://techcommunity.microsoft.com/blog/machinelearningblog/an-introduction-to-llmops-operationalizing-and-managing-large-language-models-us/3910996)

[FLAKY_TESTS]: https://testing.googleblog.com/2016/05/flaky-tests-at-google-and-how-we.html
[SAMSUNG_INCIDENT]: https://cybernews.com/news/chatgpt-samsung-data-leak/
[PII_FILTER]: https://learn.microsoft.com/en-us/azure/ai-services/language-service/personally-identifiable-information/how-to-call
[TOKEN_ESTIMATION]: https://learn.microsoft.com/en-us/azure/ai-services/openai/how-to/quota?tabs=rest#understanding-rate-limits
[OVERSHARING_BLUEPRINT]: https://learn.microsoft.com/en-us/copilot/microsoft-365/microsoft-365-copilot-blueprint-oversharing
[OWASP_LLM]: https://owasp.org/www-project-top-10-for-large-language-model-applications/
[MITRE_ATLAS]: https://atlas.mitre.org/
[MANAGING_LLM_THREATS]: https://fizzylogic.nl/2024/04/12/securing-ai-building-safer-llms-in-the-face-of-emerging-threats
[AZ_CONTENT_SAFETY]: https://learn.microsoft.com/en-us/azure/ai-services/content-safety/overview
[GA_CONTENT_SAFETY]: https://cloud.google.com/natural-language/docs/moderating-text
[AWS_CONTENT_SAFETY]: https://aws.amazon.com/solutions/ai/content-moderation/