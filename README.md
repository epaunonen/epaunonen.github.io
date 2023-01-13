# Data and Programming Portfolio
<p>This portfolio is a compilation of notebooks, dashboards and other projects which I have created for data analysis, exploring machine learning or a specific application. A summary of each project as well as the applicable learning goals are provided with the projects. The projects are categorized by type:</p>


| Power BI & Data Processing <img width=100/> | | <img width=307/> |
| --- |:-----:|:---:|
| NFL Analysis Report and Pipeline  | [Link](https://github.com/epaunonen/epaunonen.github.io/blob/main/README.md#power-bi-nfl-analysis-report-and-pipeline) | *Power BI*, *AWS*, *SQL*, *Python* |


| Machine Learning <img width=170/> | | |
| --- |:-----:|:---:|
| Airbnb Location Score Prediction | [Link](https://github.com/epaunonen/epaunonen.github.io/blob/main/README.md#airbnb-location-score-prediction) | *Python*, *Classification*, *Clustering*, *Regression* |
| Room Occupancy Classification | [Link](https://github.com/epaunonen/epaunonen.github.io/blob/main/README.md#room-occupancy-classification) | *Python* |


| Standalone Projects <img width=155/> | | <img width=307/> |
| --- |:-----:|:---:|
| Python Telegram bot for event submissions | [Link](https://github.com/epaunonen/epaunonen.github.io/blob/main/README.md#lihys-little-helper-python-telegram-bot-for-event-submissions) | *Python*, *Google APIs*, *AWS* |
| FixAr: Fixed-point arithmetic library for C# | [Link](https://github.com/epaunonen/epaunonen.github.io/blob/main/README.md#fixar-fixed-point-arithmetic-library-for-c) | *C#* |

<br>

---

# Dashboards, Reports and Data Processing

### Power BI: *NFL Analysis report and pipeline*

[**View Report**](https://app.powerbi.com/view?r=eyJrIjoiYmMyYWY2ZjgtNGM1ZC00ZGVjLWFhODMtYTY5OTM0N2I1YmJmIiwidCI6IjhkZWQ3ODVjLTJiYTYtNGIxYS05NmUyLWY3NGFiZTk2MWFiZCIsImMiOjh9)
<br><br>
![NFL Report game view](https://github.com/epaunonen/epaunonen.github.io/blob/main/Assets/NFL/NFL_1.PNG?raw=true "Game view")

This Power BI report is a comprehensive tool for monitoring and comparing NFL team performance, analyzing play type (e.g. running play, passing play) likelihood, predicting yard gains per play and viewing the progress of any NFL game from 1999 to the present day - play-by-play.<br>
<br>
This project utilizes data releases provided by [nflverse](https://github.com/nflverse/nflverse-data/releases), a NFL data project that provides up-to-date play-by-play data from NFL games.
The dataset is deployed on a simple PostgreSQL database running on Amazon Lightsail. [A Python script](https://github.com/epaunonen/epaunonen.github.io/blob/main/NFL/db/lambda_update_pbp.py) is deployed on AWS Lambda and scheduled to run byweekly during an NFL season. The script refreshes the play-by-play data in the database from the nflverse releases and then Power BI respectively refreshes the data from the database via a PBI gateway service running on Amazon EC2. This keeps the dataset and the report up-to-date ensuring that new games are available as soon as possible. During offseason, the script(s) can be used to rebuild the database in case, for example, variables are added or removed.

Learning goals for this project:
 * Learning to use Power BI (and as an extension, DAX) for efficient and diverse report creation. 
   - Dealing with a large, and quite challenging, dataset.
 * PostgreSQL database deployment and management.
 * Managing data in the database with Python and SQL
 * Creating a complete pipeline, from original datasource to a database and from there to Power BI, with automatic refreshes and monitoring
   - Deploying and managing services on AWS (Lambda, EventBridge, EC2, Lightsail)

<br>

---

# Machine Learning

### Airbnb Location score prediction
[**View Notebook on GitHub**](https://github.com/epaunonen/epaunonen.github.io/blob/main/Notebooks/Airbnb%20location%20score%20prediction/location_score_prediction.ipynb) | [**nbviewer**](https://nbviewer.org/github/epaunonen/epaunonen.github.io/blob/main/Notebooks/Airbnb%20location%20score%20prediction/location_score_prediction.ipynb)
<br><br>
![Airbnb ML](https://github.com/epaunonen/epaunonen.github.io/blob/main/Assets/Airbnb/airbnb1.PNG "Result")

In this project Airbnb data from Italy is explored, processed and analyzed with the goal of creating a model that predicts the location review score of an Airbnb listing. Airbnb listings are ranked based on 7 different review score types, one of which is the location. The analysis shows that customers tend to assess the location somewhat independent of the other categories (which are heavily correlated with each other) meaning that an accurate prediction of a location score can be used as a decision making tool when e.g. purchasing property to be listed on Airbnb.

Possibilities for further improvement are also identified in the analysis.

**Methods used:**
 - Naive methods: city and neighbourhood means
 - Clustering: KMeans, Gaussian Mixture
 - Regression: XGBoost
 - Classification (binary and multiclass): Random Forest, XGBoost

---
### Room Occupancy Classification
[**View Notebook on Github**](https://github.com/epaunonen/epaunonen.github.io/blob/main/Notebooks/Room%20occupancy%20classification/Room_occupancy.ipynb) | [**nbviewer**](https://nbviewer.org/github/epaunonen/epaunonen.github.io/blob/main/Notebooks/Room%20occupancy%20classification/Room_occupancy.ipynb)
<br><br>
![Room occupancy](https://github.com/epaunonen/epaunonen.github.io/blob/main/Assets/Room%20occupancy/img1.PNG)

This simple project was created as the final project for the course Machine Learning: Basic Principles at Aalto University. 
Here room occupancy count (0-3 people) is estimated using Logistic Regression and MLP. <br>
The aim of this project was to practice the overall process of creating a ML model, data exploration and model tuning using a easily approachable dataset. 

Further on, a more complex and realistic dataset should be considered as the sensor data present here allows for almost perfect classification with little effort whereas the sensor data available outside of a controlled experiment would likely not produce equally good results.

<br>

---

# Standalone projects

### "LiHy's Little Helper": *Python Telegram bot for event submissions*

[**View Code**](https://github.com/epaunonen/epaunonen.github.io/tree/main/Projects/LiHy's%20Little%20Helper)
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
 - Error handling: As the bot was utilized by relatively few people and the data sent through the bot was not deemed critical, I made a decision that if a picture submission failed, the bot would forward the picture as well as the error message to a specified admin account (=me). After that I would resubmit the picture successfully. This ended up working fine as only 9 pictures encountered an error (1.7% of total), but for a larger application or one handling critical data this would not be a feasible approach. Preliminary investigation suggested that these errors were caused by the Telegram API not responding even after two retries with appropriate intervals.
 - Consider not using Google Drive for storage, as the api is cumbersome and quite slow for an application like this.


<br>

---
### FixAr: *Fixed-point arithmetic library for C#*

[**View Code**](https://github.com/epaunonen/epaunonen.github.io/tree/main/Projects/FixAr%20C%23)

FixAr is a fixed point arithmetic library written in C# that is designed for fast paced simulations which require platform independent, deterministic calculations, in situations where lower level code cannot be used.

This library has been created as a hobbyist project in 2018, as a base for experimenting with simple and completely deterministic physics simulations in C#. The original motivation was to allow for deterministic calculations in Unity game engine so that the same exact results would be obtained independent of execution platform and engine version.

Here determinism is achieved by emulating decimal numbers with integers. Additionally, look-up-tables and other optimizations are used to keep the calculations as fast and light as possible to not drastically slow down the intended game loop. Due to the nature of this implementation, the calculations are not perfectly accurate which may introduce some error to the system. The precision of the number representations is configurable, but it should be noted that increasing the precision lowers the maximum values that the library can handle.

**Features**
 * Basic math library
   - Arithmetic operations
   - Trigonometry
   - Powers, logarithms
 * Vectors, 2D and 3D
 * Quaternions (partial implementation)
 * Extensive configuration options
   - Number precision
   - "Safe" function implementations that prevent overflow issues
