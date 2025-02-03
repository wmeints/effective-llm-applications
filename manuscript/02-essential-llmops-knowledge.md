{#essential-llmops-knowledge}
# Essential LLMOps knowledge

In chapter 1 we explored the basic concepts and terminology around LLMS. We briefly
touched on the essentials for building and operating LLMs. In this chapter we'll explore
the essentials of operating LLMs in more detail before we dive into working with
Semantic Kernel. Consider this chapter the moment where we start planning the LLM-based
applications we're building in the upcoming chapters.

To be fair with you, I thought of showing your Semantic Kernel first before talking
about operating LLMs. But I find the knowledge in this chapter important to learn first
because it's easy to get lost in the details of implementing various patterns with an
LLM and loose sight of the bigger picture. You will need to plan some things out for
yourself before you start building. Learning how to send a prompt to an LLM is one
thing, but learning how to manage the lifecycle of an LLM-based application is another.
And you need both to be succesful.

This chapter will help you understand the basic operations involved in building and
hosting LLM-based applications in greater detail.

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

Let's first cover what LLMOps is compared to the other types of Ops approaches out there.

## What is LLMOps?

Most of you will already know what DevOps is, it's the combination of development and
operations practices to optimize the process of building, testing, and deploying
software. Often the term DevOps is mixed with Continuous Delivery. The term DevOps has
become so popular that people started using the term MLOps to refer to similar practices
for managing the lifecycle of machine learning projects. And now that we have LLMs
people have started to use the term LLMOps to refer to the practices involved in
managing LLM-based projects.

When I talk about LLMOps I like to take a layered approach as shown in [#s](#llmops-layering).

{#llmops-layering}
![Layering DevOps, MLOps, and LLMOps](figure-1-llmops.png)

LLM-Based projects are software-based projects and should follow DevOps practices. Most
of the time the LLM only makes up a small part of the project, so most of your time will
go into managing the DevOps aspects of your project.

Since LLMs are machine-learning models it is useful to apply MLOps practices to a
project that uses an LLM. So let's explore what that means. MLOps is about managing the
lifecycle of machine learning models. It's about managing the data, the model, the
configuration, and the code to produce and use the model. When you apply MLOps you
typically:

- Track experiments to train, test and evaluate the model.
- Monitor the model in production to measure how circumstances change so you can retrain
  the model to improve it over time.
- Manage the configuration of the model to ensure that it's always up-to-date and
  running in the right environment.

These concepts apply for LLMs too. Except that it's extremely unlikely you're going to
train your own LLM. If you want to go very advanced you may fine-tune one, but I
doubt most of you will ever do that. So the focus of LLMOps is more on managing the
interaction between the LLM and the rest of your application.

When you look on the internet you'll find many definitions of LLMOps and even dedicated
tools to help you manage LLMs. But I like to stick to the basics as most LLMOps tools
aren't very mature yet.

I've found that the following 7 aspects deserve special attention when building and
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

Testing LLM-based applications is a bit different from testing regular software
applications. In regular software applications you can use tests that assert a true or
false statement. Typically, you'll write a combination of unit-tests, integration tests,
and possibly even acceptance tests to validate that your application is working as
expected using a boolean approach to verify that some rules hold true for your system.

LLM-Based applications have to deal with a component (the LLM) that can't be tested with
the same types of rules as other software. It isn't as black and white. LLMs give you a
different response every time, because of what we discussed in chapter 1, they use
output sampling techniques. You need a different testing approach validate that your
LLM-based application is working.

Where unit-tests and integration tests should be a standard practice already, for
LLM-based applications you need to apply one extra layer of tests. LLM-based
applications need non-deterministic tests. Which sounds weird, because why would you
want a test for which you aren't getting a yes or no? It will be hard to not end up with
[flaky tests][FLAKY_TESTS].

The reason is that you're not looking for an absolute answer, you're not going to get
it. Instead, you want the right answer, most of the time. We're trying to lower the risk
that our solution poses as much as possible. And we need to accept that just like with
regular software, sometimes the system fails for unknown reasons. But for an LLM-based
application we at least have a good idea where the problem is coming from.

One testing strategy that works great in an LLM-based is to run multiple samples through
the application and then have the test cast a majority vote to determine if the call to
the LLM is producing the correct output.

Another great strategy is to use metrics with a value range to determine that the output
is within acceptable limits for your choosen metric.

I can imagine that this generates more questions than answers. For now, it's important
to understand that you need to change your perspective on testing. Throughout the
remaining chapters of this book I'll show you how to test LLM-based applications in more
detail:

- In [#s](#the-art-and-nonsense-of-prompt-engineering), we'll look at how to use prompt
  testing as a form of unit-testing for your prompts.
- In [#s](#prompt-testing-and-monitoring), we'll use integration testing to validate that the LLM can make tool
  calls by applying integration tests.
- In [#s](#enhancing-llms-with-tools), we dive into integration tests for a combination
  of search and an LLM.

{#llmops-monitoring}
## Monitoring and evaluation of your application

You can consider testing as a way to make sure you have a safety net in place to catch
problems before to happen in production. Monitoring is the second safety net you need to
catch problems while running in production.

There are two important aspects to monitoring LLM solutions. It should come as no
surprise that you need to monitor for any infrastructure issues and technical problems
with your code. I consider this more of a DevOps practice than an LLMOps practice.

The LLMOps approach to monitoring focuses on the interaction between your application
and the LLM. Machine-learning models like LLMs tend to fail silently in production. As
the model is used, people change their interaction pattern and the model starts to
generate less accurate responses. This is called model drift or concept drift.

For interactions between the LLM and external data sources when applying patterns like
Retrieval Augmented Generation (RAG) it is important to track those interactions. As
information changes in your data sources, the LLM may start to generate less accurate
responses. You could call this data drift.

We'll cover monitoring in greater deal as we go through the remaining chapters in this book:

- In [#s](#the-art-and-nonsense-of-prompt-engineering), we use monitoring to collect data needed to iterate on your prompts.
- In [#s](#prompt-testing-and-monitoring), we cover how to monitor calls to tools connected to the LLM.
- In [#s](#enhancing-llms-with-tools), we'll look at how to monitor the interaction between the LLM and a search engine.

Monitoring interactions is essential to keep your LLM-based application running smoothly in production.
And while your setting up monitoring, I recommend including monitoring costs as well. 

## Cost management and optimization

Cost management is a big part of LLMOps. When you host an LLM-based application in the
cloud you can run into high costs very quickly if you build just the right kind of logic
so that a workflow never ends or becomes very inefficient because the LLM generates
weird output that causes the worklow to make the wrong decision.

Let's imagine you built an LLM-based application that can decide if a text is of enough
quality to be used by the user. If it's not, it can go back and improve the text based
on feedback it generated using the LLM. This sounds like a great idea, which we'll cover
in chapter 11, but it comes with a major downside. When will the loop end? If it's good
enough? Well, maybe, but what if the LLM generates output that keeps asking for more
changes? You can't just let the workflow run forever, because it will cost you a lot of
money. You need to think about how you can limit the cost of running an LLM-based
application.

The answer lies in two things: Limit how much money you're willing to burn
on the loop in your application, and setting a hard limit on how many times the agent is
allowed to try to improve the text.

In chapter 1, we covered various LLM providers to choose from. Each has their own cost
management solution, which makes it hard to give you a general tutorial on how to manage
costs. Instead, let me point you towards the documentation of the provider you're using.
They will have a section on cost management:

- [OpenAI](https://platform.openai.com/usage) lets you set a monthly limit for your
  organization. This is a hard limit which is reset every month.
- [Microsoft
  Azure](https://learn.microsoft.com/en-us/azure/cost-management-billing/manage/spending-limit)
  Let's you set a spending limit for your subscription. Additionally, you can set a
  token per minute limit for the LLM to slow things down.
- [Google](https://cloud.google.com/billing/docs/how-to/budgets) doesn't have hard
  spending limits. Instead, you need to spend more money on a budget alerting solution
  that invokes the billing API to stop the billing for your project.
- [Anthropic](https://docs.anthropic.com/en/api/rate-limits) lets you set a rate limit
  and spending limit for your organization. This limit is reset every month.

As a general rule I recommend setting a spending limit that's reasonable for your
project. Start on the lower end so you don't let things get out of hand too early. Run a
test with a small group to establish a benchmark for the costs and then adjust the
spending limit and rate limit accordingly. This way you can keep the costs under
control.

Now that we have the important monitoring concepts in place, let's move on to dealing with
failure modes in your LLM-based application.

{#llmops-rate-limits}
## Dealing with rate limits and capacity planning

All LLM providers are dealing with an enormous influx of people who want to use their
services. Currently, it's impossible for all of them to get the amount of GPU power they
need to fulfil all the requests. You're going to have to deal with that.

The LLM providers introduced rate limits to control the traffic flow into their data
centers, and you will hit those limits very quickly I've found.

For some providers like Azure, you can assign a quota to your LLM instance. This quota
is a token budget that your application can spend per minute to make calls to the LLM.

It's important to note here that the quota is not a hard limit or a guarantee for how
fast you'll get a response. An LLM provider makes [an estimation][TOKEN_ESTIMATION] of
how many tokens you need for a request. It then processes the request if it fits in the
budget.

If you run out of tokens, you'll have to wait a bit before you can make another call.
The best way to work with rate limits is to have a retry mechanism with an exponential
backoff. If you do this correctly, it's another layer of infrastructure code that
doesn't get in the way of you building your application.

Usually you'll find that you get higher speeds when you set a higher quota. But it's best
to measure how many tokens you typically need in your application and set the value for
the quota to a reasonable value. You can always increase the quota if you need more.

When I can, I'll take a look at monitoring data during early testing to understand how
many tokens per minute we need and set the quota accordingly. You need to strike a
balance between costs and user experience here, because rate limits help you control
costs too. The lower the quota, the lower the amount of tokens that can be spent per
minutes, the lower the bill. However, too many retries can lead to a bad user
experience.

In chapter 4, we'll cover how to configure parameters like `max_tokens` and `best_of`
for your prompt to make the most out of your quota.

## LLM performance and the user experience of your application

I already mentioned performance in the previous section, and it's a thing for LLM-based
applications. LLMs are very slow, and you need to think about how you can work around
that limitation, because you can't speed them up yourself. In some cases you can
increase rate limits to speed things up a little, but that's about it.

Let me shift your perspective a bit about performance in software applications. Under
normal circumstances you need to look for performance bottlenecks in all areas of your
application. And it's likely you can solve a lot of those bottlenecks yourself. When
using an LLM in your application it is important to know that the LLM will your biggest
bottleneck, and you can't speed it up by much.

There are two things that you need to think about when optimizing LLM-based application
performance:

- First, you need to think about ways to improve the user experience. For example, you
can let the LLM stream the response to a prompt. This way the user receives an answer in
chunks, but quicker.
- Second, you need to think less about the performance of the code around the call to an
LLM. It won't matter as much what you do there, because the LLM is the slowest piece in
the puzzle.

Do keep in mind that you'll want to optimize the rest of the code that doesn't call the
LLM to be fast, otherwise you're just adding to the problem. And I've found that if the
rest of the application is nice and fast, users are more forgiving about the LLM being
slow.

{#llomops-failover-strategies}
## Failover strategies

As if performance wasn't hard enough, we get another nice present from the LLM
providers. LLM providers are sometimes unavailable because you've hit a hard limit, or
because they're overloaded. Anthropic has this problem quite frequently when I wrote
this chapter where you receive overloaded errors and can only call the LLM again after a
few hours. This can be quite problematic when you run a production application that
needs some form of availability.

In many cases I've found it's enough to let the user know that the LLM is unavailable if
you haven't any sort of availabiliy requirements. Many of my clients don't have any
requirements for availability because they're just starting out and know that they can't
guarantee 100% uptime.

I learned from experience that despite the lack of availability requirements it's
beneficial to have a failover strategy in place. For example, you could have GPT-4o as a
fallback for Claude Sonnet in your application. This way you can switch to GPT-4o when
Claude Sonnet is unavailable.

Regardless of what your client thinks, it's good to spend some time planning for
failover scenarios as this has a big impact on the user experience and the development
effort you need to spend.

{#llmops-security}
## Security and privacy in an LLM-based application

We've covered quite a few LLMOps essentials. And you may have noticed that not many of
these aspects are well understood by the users of LLM-based applications. This is
especially true for security and privacy concerns.

I've not had many security discussions with clients about LLMs. I think this is because
we're just getting started using LLMs in production. Not many people realize how
dangerous LLMs are. I could write an entire book about this topic, but I'll have to keep
this a little shorter, because we need to talk about other things. But let me give you
the basics so you know what you're getting yourself into.

There are three main topics when we talk about security in relation to LLMs:

- **Data privacy:** Customers and users will talk about data privacy, because they're
  worried about the provider stealing their data for training the LLM.  
- **Application security:** This is about how you can protect your application from
  being hacked. This is a big topic, because LLMs are very powerful and can be used to
  generate malicious content.  
- **User safety:** This is about how you can protect your users from being exposed to
  harmful content generated by the LLM. LLMs can generate harmful content or misleading
  content that can be used to manipulate people.

Let me go over each of these topics to give you a direction on how to think about them.

{#llmops-data-privacy}
### Data privacy

Before you start using an LLM you need to think about the data you're sending to the
LLM.

The [Samsung incident][SAMSUNG_INCIDENT] teached us an important lesson that if
your data is going to be used for training it is likely to end up on someone else's desk
at some point in the future. Hackers are actively probing with prompts to find out what
data is being used to train the LLM. 

Sometimes it's better not to use an LLM if you're worried about company secrets being
leaked. For most cases however it's good enough to set up an agreement with the LLM
provider and ask it not to use your data for training. All LLM Providers offer this
option today and have a section about it in their legal agreements.

I find it important to understand that security is a trade-off. You can't have 100%
security, because then you wouldn't use LLMs in the first place and miss out on their
value. However, you can't completely live without security either. I recommend you have
an active discussion with your client about the data you're sending to the LLM and the
risks involved and then make a decision based on that.

There's another closely related topic to data privacy that I want to address here. In
more and more countries there are laws that require you to be careful with personal
identifiable information (PII). For example, in Europe you need to comply with GDPR. In
California, you need to comply with CCPA. These laws require you to be careful with the
data you're processing.

Managing personal data is a topic that you should address alongside the other security
requirements when planning an LLM-based application.

Many cloud providers offer standard tools to filter out PII from text. For example,
Azure has a tool called [Text Analytics][PII_FILTER] that can be used to filter out PII
from text. This way you can avoid sending PII to the LLM or storing it in your
application. Consider it an extra safety measure on top of making sure people know that
they're not supposed to send in PII.

{#llmops-application-security}
### Application security

Securing modern cloud-native applications is difficult. Adding an LLM to the mix opens
up a new chapter in application security for sure. And it's a relatively new chapter, so
there's not much information available on how you can secure an LLM-based application. I
will cover some LLM security strategies around security in greater detail in the next
chapters, but I want to make sure you have a good overview of the topic first.

The best way to handle application security is to apply a deep defense strategy. This
means that you'll want to apply multiple layers of defences to ensure that if a hacker
gets through one layer, it will have to work hard to get through the next layer. A
layered defense also helps to keep the blast radius of an attack to a minimum.

One of the best moves you can make in terms of application security is to not give your
application access to resources that it shouldn't have access to. I see a lot of people
using Copilot 365 and overshare information with it because of bad defaults and the
admin not being aware that you should be careful. Microsoft even made a
[blueprint][OVERSHARING_BLUEPRINT] to help you fix the issues.

If there's one thing you should learn from this, it should be this: Make deliberate
choices in what you hand to an LLM. It's better to start with nothing.

If your application needs to use a database, don't give it admin access to
the database. It does sound like old advice, and it is, but it is extra relevant now
that we can let the LLM generate SQL queries and other nasties. You don't want to give
the LLM the ability to drop tables in your database. I don't even recommend letting the
LLM generate SQL queries and then run those queries.

If you're interested in learning more about application security in relation to
application security, I highly recommend looking at the following two resources:

- [OWASP Top 10 for LLM applications][OWASP_LLM]
  - Covers the top 10 most occuring security risks in LLM-based applications.
- [MITRE Atlas][MITRE_ATLAS] - Covers all possible attacks againt AI systems

I also recommend that you adopt a security-first mindset when developing LLM-based
applications. You can do this with just a few steps. I wrote a blog post about this
topic that you can read [here][MANAGING_LLM_THREATS].

While application security is a large portion of the security concerns you'll have to
deal with, there's another part that's equally important. User safety.

{#llmops-user-safety}
### User safety

We've all seen the videos and articles on internet showing LLMs generating harmful or
plain silly content. It's a real concern if you ask me, because we seem to attribute
this to the LLM's limitations while it's actually the developers not thinking about the
consequences of their application's output.

We often call the weird output of an LLM a hallucination, but that's not really what's
happening. Let's take a step back to how an LLM works to fully understand what I mean
with this statement.

When you ask an LLM to generate output for a prompt, the LLM predicts the most likely
text that follows your prompt as we covered in chapter 1. The LLM does this by using an
attention mechanism combined with an embedding layer.

The embedding layer gives meaning to what is the most relevant in terms of context
surrounding a token in the context that we're looking at. The attention mechanism is
trained to focus on the most relevant parts of the context we're working with.

Both the embeddings and the attention mechanism are trained on all sorts of data,
including harmful content. It is impossible to filter out the harmful training content,
because we're talking about trillions of tokens used to train LLMs. This means that the
LLM can generate harmful content, because it has seen it before, and it's essentially
parrotting the content it's seen before. It's just plain math.

For you as a developer of an LLM-based application this means that you need to be aware
of this and take other measures to keep your users safe. You can't fix the model, so you
need to fix something else. If you want to protect your user against malicious content,
you'll need to apply output sanitation not only from a technical perspective, but also
from a content safety perspective.

Lucky for us, there are tools to detect malicious content and filter it out. For
example, Azure has a tool called [AI Content Safety][AZ_CONTENT_SAFETY]. Google has the
same feature called [text moderation](GA_CONTENT_SAFETY). Finally, AWS has a solution
called [content moderation][AWS_CONTENT_SAFETY] too. So it's not a lot of work to
integrate content filtering even when you're unsure if your application will generate
harmful content.

## Considerations when looking for tools

When you browse the internet on a regular basis and look at social media like LinkedIn,
X, and BlueSky, you'll see a lot of ads about LLMOps tools. I personally don't use any
of these tools right now, because their value isn't clear to me yet. There are existing
tools for a lot of the aspects mentioned in this chapter and I recommend readin the rest
of this book before you start browsing for an LLMOps solution. You'll have a better
understanding of what you need and what you don't need.

I do want to give you a few pointers on what to look for when you're looking for an
LLMOps tool:

- As tools are evolving quickly you'll likely switch one or two times to a newer tool in
  the next year. So make sure you choose a tool that you can easily migrate away from.
- Make sure that the tool has the right measures in place to protect your data. It's
  silly to expose your data through the LLMOps provider while worrying about the LLM
  itself. Make sure the tool has a good security track record.
- Make sure the tool has a good support team. You'll likely run into issues with the
  tool, because it's new, and you're new to it. You'll need a good support team to help
  you out.

For most teams that I work with it is important to integrate the LLM aspects into the
rest of their DevOps practices so they keep their mental model simple. I recommend you
do the same until a clearer picture of LLMOps tools emerges.

## Summary

In this chapter we covered the essentials of LLMOps. We discussed the differences
between regular software projects and LLM-based projects. We talked about the importance
of testing LLM-based applications and monitoring and evaluating your application. We
also covered cost management and optimization, rate limiting and capacity planning,
performance considerations, failover strategies, security and privacy concerns, and
considerations when looking for tools.

In the next chapter we'll dive into Semantic Kernel, a tool that I've been using to
build LLM-based applications to learn what it looks like and how you can set up a
project with it so we can start learning about LLM-based workflows and agents in later
chapteres.

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