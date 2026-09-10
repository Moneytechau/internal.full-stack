# Preparation

You will complete a task where you modify an application that consists of an angular front end and a .Net backend.

Ensure you have:
- dotnet 10 installed
- npm version 11+ installed
- Chrome/Chromium based browser for playwright tests

During the test you will share your screen as you work on completing the task. During the test you can:
- use AI tools to investigate the code or implement the code
- google for solutions

You should come to the test prepared to write code by hand or with AI assistance.

We recommend using vscode, although Visual Studio or Rider should work fine.

# Structure of the test

There are 3 parts to the test:
- 10 minutes - prepare to complete the task. Take the time to look at the solution and the test. You can ask any clarifying questions during this time. Use this time to get ready to complete the task.
- 40 minutes - work to complete the task.
- 10 minutes - discuss your solution to the task.

# The task to implement

Your task is to add an additional step into the existing application. The application is currently a 3 step process to register an incident. We want to add an additional step that provides a summary of the incident that is being reported.

Requirements:
- We want to add the additional option of selecting a currency for the incident damage in the one of the following currencies (AUD, USD, GBP). The currency should be collected on step 2 with the estimated damage.
- We want to add a 4th step which is a summary page. On the summary page it should display all of the information collected. On the summary page it should display the estimated damage in AUD. Use the https://frankfurter.dev/ API to retrieve the exchange rate at the time of the accident, if the estimated damage was in USD or GBP.
- Ensure that the final incident damage value in AUD is saved, along with the originally reported value and currency.