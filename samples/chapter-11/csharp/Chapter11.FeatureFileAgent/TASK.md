## Purpose 

The purpose of the agenttalks application is to help us gain insights into what's happening
in AI without having to read all the content that's available to us. The application
will summarize content for us and produce a weekly podcast for us to listen to so we
get a quick overview of about 6 minutes rather than reading for multiple hours. We'll
provide original sources with the podcast so we have access to the full information if
we find that we want to learn more about a particular article.

## Background

The agenttalks solution was invented because we are overloaded with content. Joop and I
send eachother links to interesting content. We do this almost every day sometimes
multiple times per day. This leads to an increasing amount of content that we need to
get through. Often, we share scientific content that requires more reading time. This
increases the burden on us to get through it.

It's important to understand why we read so much content. We want to stay on top of AI
so that we can help our colleagues be productive without having to figure everything out
by themselves. 

We figured that if we want to keep getting through a large amount of information we
needed a solution that summarizes the content. We then figured it would be useful to
have a podcast every week that we can listen to so we can get the information while in
a car or doing other activities.

## Flow of the application

### Submitting content for the podcast

Users can submit a URL through a form that is then summarized by the application for inclusion in the podcast.
The application extracts key discussion points and actionable insights from the content. We support PDF, and plain HTML content for now.

### Podcast generation

Every friday, a timed workflow takes the summarized content, creates a podcast script, and then generates audio based on the script.
The podcast audio is then published through buzzsprout as a new podcast episode for people to listen to.