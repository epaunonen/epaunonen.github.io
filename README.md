# Projects
<p>This portfolio will be a compilation of notebooks, dashboards and other standalone projects which I have created for data analysis, exploring machine learning or a specific application. The projects are divided by category.</p>

## Power BI Dashboards & Reports

### NFL Analysis

[View Report](https://app.powerbi.com/view?r=eyJrIjoiYmMyYWY2ZjgtNGM1ZC00ZGVjLWFhODMtYTY5OTM0N2I1YmJmIiwidCI6IjhkZWQ3ODVjLTJiYTYtNGIxYS05NmUyLWY3NGFiZTk2MWFiZCIsImMiOjh9)
<br><br>
![NFL Report game view](https://github.com/epaunonen/epaunonen.github.io/blob/main/Assets/NFL/NFL_1.PNG?raw=true "Game view")

This Power BI report is a comprehensive tool for monitoring and comparing NFL team performance, analyzing play type (e.g. running play, passing play) likelihood, predicting yard gains per play and viewing the progress of any NFL game from 1999 to the present day - play-by-play.<br>
<br>
This project utilizes data releases provided by [nflverse](https://github.com/nflverse/nflverse-data/releases), a project dedicated to NFL data.<br>
The dataset is deployed on a simple PostgreSQL database running on Amazon Lightsail. Power BI periodically refreshes the data from the database via a PBI gateway service. A [python script](https://github.com/epaunonen/epaunonen.github.io/blob/main/NFL/db/update_pbp.py) is deployed on AWS Lambda and scheduled to run regularly during an ongoing NFL season. The script refreshes the play-by-play data in the database from the nflverse releases, which keeps the dataset and the report up-to-date ensuring that new games are available as soon as possible. During offseason, the script(s) can be used to rebuild the database in case, for example, variables are added or removed.

The learning goal of this project was to learn how to use Power BI, and especially DAX, for efficient report creation with a large dataset. An additional topic of interest has been the automation of data refreshes, from the original data source to the database and from there to the Power BI report. 

## Standalone projects

### "LiHy's Little Helper" - Telegram bot for event submissions (Python)

[View Code](https://github.com/epaunonen/epaunonen.github.io/tree/main/Projects/LiHy's%20Little%20Helper)
<br><br>
![LiHyBot](https://github.com/epaunonen/epaunonen.github.io/blob/main/Assets/LiHyBot/LLH.PNG?raw=true "Telegram Bot")

This bot was created for a student-run wellbeing event that ran from June to August 2022. The idea of the event was to make the participant document their summer through pictures. A Telegram chatbot was chosen as a submission platform as it is easily available for everyone, quick to use for picture submissions and allows for a neat UI implementation.
<br><br>
The bot was developed using Python with [python-telegram-bot](https://github.com/python-telegram-bot/python-telegram-bot) as a wrapper for the Telegram bot api. As a feature request by the event organizers the bot used Google Drive, with drive api through a specifically created Google Service Account, as storage for the submissions. This was done to allow the organizers to conveniently access the pictures for verification. The bot was deployed to an Amazon EC2 instance, which was configured to automatically restart the process if a critical error occurred - a failsafe that was not triggered at all during the event.

**Stats:**
 - 55 unique users
 - 542 pictures submitted in total
 - 71 days

**Things I would do differently next time:**
 - Error handling: As the bot was utilized by relatively few people and put together very quickly and the data sent through the bot was not deemed critical, I did not thorougly prepare for errors. I made a decision that if a picture submission failed, the bot would forward the picture as well as the error message to a specified admin account (=me). After that I would manually submit the picture again. This ended up working fine as only 9 pictures encountered an error (1.7% of total), but for a larger application this would be unacceptable. It should be noted that all these errors were caused by the Telegram API not responding even after two retries.
 - Consider not using Google Drive for storage, as the api is cumbersome and quite slow for an application like this.



---
### FixAr - Fixed-point arithmetic library for C#

[View Code](https://github.com/epaunonen/epaunonen.github.io/tree/main/Projects/FixAr%20C%23)

FixAr is a fixed point arithmetic library written in C# that is designed for fast paced simulations which require platform independent, deterministic calculations, in situations where lower level code cannot be used.

This library has been created as a hobbyist project in 2018, as a base for experimenting with simple and completely deterministic physics simulations in C#. The original motivation was to allow for deterministic calculations in Unity game engine so that the same exact results would be obtained independent of execution platform.

Determinism is achieved by emulating decimal numbers with integers. Additionally, look-up-tables and other optimizations are used to keep the calculations as fast as possible. Due to the nature of this implementation, the calculations are not perfectly accurate which may introduce some error to the system. The precision of the number representations is configurable, but it should be noted that increasing the precision lowers the maximum values that the library can handle.

**Features**
 * Basic math library
   - Arithmetic operations
   - Trigonometry
   - Powers, logarithms...
 * Vectors, 2D and 3D
 * Quaternions (partially)
 * Extensive configuration options
   - Number precision
   - "Safe" function implementations that prevent overflow issues
