# HighDatabaseConnections

MEDIUM 

## Description

Alerts when any of the API database exceeds 75 out of a possible 100 connections.

## Potential Causes

In the past we’ve seen this happen for two reasons:

1. [The Hangfire Postgres provider was leaking database connections](https://github.com/DFE-Digital/get-into-teaching-api/pull/101).
2. [The number of Hangfire workers was exhausting the connection pool](https://github.com/DFE-Digital/get-into-teaching-api/pull/97).

## Resolutions

[Check the Grafana panel for an indication of what’s going on](https://grafana-prod-get-into-teaching.london.cloudapps.digital/d/28EURzZGz/get-into-teaching-api?viewPanel=4&orgId=1&var-App=get-into-teaching-api-prod).

Look for any recent changes in the application source code or it’s dependencies that may be causing this; it’s usually a tricky one to pin down and the cause could be obscure, so it will just need investigating. If it’s causing issues in production and you can safely revert to before it was occurring then I would advise that course of action initially.
