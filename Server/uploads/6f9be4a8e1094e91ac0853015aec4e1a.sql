-- √руппа 1: ¬рачи 1Ц3 ? ѕн и ¬т
INSERT INTO ReceptionTimetable (DoctorID, Weekday, ReceptionStartTime, ReceptionEndTime)
SELECT 
    DoctorID,
    weekday,
    startTime,
    DATEADD(HOUR, 3, startTime) AS endTime
FROM (
    VALUES
    (1, 1, '09:00'), (1, 2, '09:00'),
    (2, 1, '12:00'), (2, 2, '12:00'),
    (3, 1, '15:00'), (3, 2, '15:00')
) AS Schedule(DoctorID, weekday, startTime);

-- √руппа 2: ¬рачи 4Ц6 ? —р и „т
INSERT INTO ReceptionTimetable (DoctorID, Weekday, ReceptionStartTime, ReceptionEndTime)
SELECT 
    DoctorID,
    weekday,
    startTime,
    DATEADD(HOUR, 3, startTime)
FROM (
    VALUES
    (4, 3, '09:00'), (4, 4, '09:00'),
    (5, 3, '12:00'), (5, 4, '12:00'),
    (6, 3, '15:00'), (6, 4, '15:00')
) AS Schedule(DoctorID, weekday, startTime);

-- √руппа 3: ¬рачи 7Ц10 ? ѕт, —б, ¬с
INSERT INTO ReceptionTimetable (DoctorID, Weekday, ReceptionStartTime, ReceptionEndTime)
SELECT 
    DoctorID,
    weekday,
    startTime,
    DATEADD(HOUR, 3, startTime)
FROM (
    VALUES
    (7, 5, '09:00'), (7, 6, '09:00'), (7, 7, '09:00'),
    (8, 5, '12:00'), (8, 6, '12:00'), (8, 7, '12:00'),
    (9, 5, '15:00'), (9, 6, '15:00'), (9, 7, '15:00'),
    (10, 5, '10:00'), (10, 6, '14:00'), (10, 7, '16:00') -- немного другой график
) AS Schedule(DoctorID, weekday, startTime);