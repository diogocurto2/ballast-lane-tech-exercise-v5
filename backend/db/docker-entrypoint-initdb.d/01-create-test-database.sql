-- The application database (taskmanager) is created by the image entry point.
-- Each test suite gets its own database so suites running in parallel never truncate or assert on
-- another suite's rows.
CREATE DATABASE taskmanager_test;
CREATE DATABASE taskmanager_test_userapi;
CREATE DATABASE taskmanager_test_taskapi;
CREATE DATABASE taskmanager_test_migrations;
CREATE DATABASE taskmanager_test_postgres;
