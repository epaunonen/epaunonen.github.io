# Projects
<p>This portfolio will be a compilation of notebooks, dashboards and other standalone projects which I have created for data analysis, exploring machine learning or a specific application. The projects are divided by category.</p>

## Power BI Dashboards

### NFL Analysis

[View Dashboard](https://app.powerbi.com/view?r=eyJrIjoiYmMyYWY2ZjgtNGM1ZC00ZGVjLWFhODMtYTY5OTM0N2I1YmJmIiwidCI6IjhkZWQ3ODVjLTJiYTYtNGIxYS05NmUyLWY3NGFiZTk2MWFiZCIsImMiOjh9)
<br>
<br>![NFL Dashboard game view](https://github.com/epaunonen/epaunonen.github.io/blob/main/Assets/NFL/NFL_1.PNG?raw=true "Game view")

This dashboard offers functionality for comparing NFL team performance and play type (e.g. running play, passing play) likelihood and yard gains.
Additionally, it allows for a detailed view of any NFL game from 1999 to present day.
Functionality for predicting the outcome of a single play with full parameter control is planned, but not yet at a working state.<br>
<br>
This project uses data provided by [nflfastR](https://www.nflfastr.com/index.html), a R package for obtaining NFL play-by-play data.<br>
At the moment, a NoSQL database is built and updated locally by the package and afterwards the dashboard is refreshed with the updated data.<br>

The learning goal of this project was to learn how to use Power BI for Dashboard creation, using a big datasource that needs a significant amount of preprocessing work in Power Query. Many of the visual heavily utilize custom DAX measures as the dataset structure necessitated this - the improved performance is an added bonus!

## Standalone projects

### "LiHy's Little Helper" - Telegram bot for event submissions

