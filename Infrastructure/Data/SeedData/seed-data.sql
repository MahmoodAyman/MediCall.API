BEGIN TRANSACTION;

-- 1. Clear existing data in correct dependency order
DELETE FROM AspNetUserRoles;
DELETE FROM AspNetRoles;

DELETE FROM VisitServices;
DELETE FROM Reviewings;
DELETE FROM Payments;
DELETE FROM Visits;
DELETE FROM NurseCertificates;
DELETE FROM PatientIllnesses;
DELETE FROM Notifications;
DELETE FROM ChatReferences;
DELETE FROM Services;
DELETE FROM Illnesses;
DELETE FROM Certificates;
DELETE FROM Patients;
DELETE FROM Nurses;
DELETE FROM AspNetUsers;

-- 2. Seed Identity Roles
INSERT INTO AspNetRoles (Id, [Name], NormalizedName, ConcurrencyStamp)
VALUES
  ('nurse',   'Nurse',   'NURSE',   NEWID()),
  ('patient', 'Patient', 'PATIENT', NEWID()),
  ('admin',   'Admin',   'ADMIN',   NEWID());

-- 3. Create VisitServices join table if missing
IF OBJECT_ID('dbo.VisitServices','U') IS NULL
BEGIN
  CREATE TABLE dbo.VisitServices (
    VisitId   INT NOT NULL,
    ServiceId INT NOT NULL,
    CONSTRAINT PK_VisitServices PRIMARY KEY (VisitId,ServiceId),
    CONSTRAINT FK_VS_Visit   FOREIGN KEY (VisitId)   REFERENCES dbo.Visits(Id),
    CONSTRAINT FK_VS_Service FOREIGN KEY (ServiceId) REFERENCES dbo.Services(Id)
  );
END;

-- 4. Seed AspNetUsers (all required Identity columns + your custom fields)
INSERT INTO AspNetUsers
(
  Id,
  UserName, NormalizedUserName,
  Email,    NormalizedEmail,
  EmailConfirmed,
  PasswordHash,
  SecurityStamp,
  ConcurrencyStamp,
  PhoneNumber,
  PhoneNumberConfirmed,
  TwoFactorEnabled,
  LockoutEnd,
  LockoutEnabled,
  AccessFailedCount,
  FirstName,
  LastName,
  DateOfBirth,
  Gender,
  Location_Lat,
  Location_Lng,
  IsDeleted,
  CreatedAt
)
VALUES
-- Ahmed (nurse, Male=0)
(
  '29901010101010',
  'nurse.ahmed', UPPER('nurse.ahmed'),
  'ahmed@example.com', UPPER('ahmed@example.com'),
  1, NULL, NEWID(), NEWID(), NULL, 0, 0, NULL, 1, 0,
  'Ahmed','Mohamed','1985-05-15', 0, 30.044420, 31.235712, 0, '2023-01-01'
),
-- Sara (nurse, Female=1)
(
  '29902020202020',
  'nurse.sara', UPPER('nurse.sara'),
  'sara.nurse@example.com', UPPER('sara.nurse@example.com'),
  1, NULL, NEWID(), NEWID(), NULL, 0, 0, NULL, 1, 0,
  'Sara','Kamal','1990-11-20', 1, 30.048941, 31.238765, 0, '2023-01-15'
),
-- Mohamed (nurse, Male=0)
(
  '29903030303030',
  'nurse.mohamed', UPPER('nurse.mohamed'),
  'mohamed.nurse@example.com', UPPER('mohamed.nurse@example.com'),
  1, NULL, NEWID(), NEWID(), NULL, 0, 0, NULL, 1, 0,
  'Mohamed','Ali','1988-07-10', 0, 30.042356, 31.233421, 0, '2023-02-01'
),
-- Ali (patient, Male=0)
(
  '30001010101010',
  'patient.ali', UPPER('patient.ali'),
  'ali@example.com', UPPER('ali@example.com'),
  0, NULL, NEWID(), NEWID(), NULL, 0, 0, NULL, 1, 0,
  'Ali','Hassan','1992-03-25', 0, 30.049543, 31.240123, 0, '2023-01-05'
),
-- Layla (patient, Female=1)
(
  '30002020202020',
  'patient.layla', UPPER('patient.layla'),
  'layla@example.com', UPPER('layla@example.com'),
  0, NULL, NEWID(), NEWID(), NULL, 0, 0, NULL, 1, 0,
  'Layla','Mahmoud','1987-09-12', 1, 30.047231, 31.237654, 0, '2023-01-10'
),
-- Omar (patient, Male=0)
(
  '30003030303030',
  'patient.omar', UPPER('patient.omar'),
  'omar@example.com', UPPER('omar@example.com'),
  0, NULL, NEWID(), NEWID(), NULL, 0, 0, NULL, 1, 0,
  'Omar','Sayed','1975-12-30', 0, 30.045678, 31.236789, 0, '2023-01-20'
),
-- Nada (patient, Female=1)
(
  '30004040404040',
  'patient.nada', UPPER('patient.nada'),
  'nada@example.com', UPPER('nada@example.com'),
  0, NULL, NEWID(), NEWID(), NULL, 0, 0, NULL, 1, 0,
  'Nada','Adel','1995-06-18', 1, 30.043210, 31.234567, 0, '2023-02-05'
),
-- Hassan (patient, Male=0)
(
  '30005050505050',
  'patient.hassan', UPPER('patient.hassan'),
  'hassan@example.com', UPPER('hassan@example.com'),
  0, NULL, NEWID(), NEWID(), NULL, 0, 0, NULL, 1, 0,
  'Hassan','Ibrahim','1968-04-22', 0, 30.041234, 31.232345, 0, '2023-02-10'
);

-- 5. Assign users to roles
INSERT INTO AspNetUserRoles (UserId, RoleId)
VALUES
  ('29901010101010','nurse'),
  ('29902020202020','nurse'),
  ('29903030303030','nurse'),
  ('30001010101010','patient'),
  ('30002020202020','patient'),
  ('30003030303030','patient'),
  ('30004040404040','patient'),
  ('30005050505050','patient');

-- 6. Seed Nurses & Patients
INSERT INTO Nurses (Id, LicenseNumber, ExperienceYears, IsAvailable, VisitCount, IsVerified)
VALUES
  ('29901010101010','RN123456', 8, 1, 42, 1),
  ('29902020202020','RN789012', 5, 1, 28, 1),
  ('29903030303030','RN345678',10, 0, 65, 1);

INSERT INTO Patients (Id)
VALUES
  ('30001010101010'),
  ('30002020202020'),
  ('30003030303030'),
  ('30004040404040'),
  ('30005050505050');

GO

-- 7. Certificates (set explicit IDs)
SET IDENTITY_INSERT Certificates ON;

INSERT INTO Certificates (Id, Name, IsRequired, IsExpirable, Description)
VALUES
  (1,'Graduation Certificate',     1,0,'Proof of completion of nursing education'),
  (2,'Work License',               1,1,'Official license to practice nursing'),
  (3,'Syndicate Card',             1,1,'Membership card of the Nursing Syndicate'),
  (4,'Criminal Record Clearance',  1,1,'Police-issued criminal record check');

SET IDENTITY_INSERT Certificates OFF;
GO

-- 8. NurseCertificates: assign all four to each nurse
INSERT INTO NurseCertificates (NurseId, CertificateId, FilePath, ExpirationDate, IsVerified)
VALUES
  ('29901010101010',1,'/certificates/graduation_ahmed.pdf',     NULL,         1),
  ('29901010101010',2,'/certificates/worklicense_ahmed.pdf',    '2026-05-01', 1),
  ('29901010101010',3,'/certificates/syndicatecard_ahmed.pdf',  '2026-05-01', 1),
  ('29901010101010',4,'/certificates/criminalrecord_ahmed.pdf', '2026-05-01', 1),

  ('29902020202020',1,'/certificates/graduation_sara.pdf',      NULL,         1),
  ('29902020202020',2,'/certificates/worklicense_sara.pdf',     '2026-06-15', 1),
  ('29902020202020',3,'/certificates/syndicatecard_sara.pdf',   '2026-06-15', 1),
  ('29902020202020',4,'/certificates/criminalrecord_sara.pdf',  '2026-06-15', 1),

  ('29903030303030',1,'/certificates/graduation_mohamed.pdf',   NULL,         1),
  ('29903030303030',2,'/certificates/worklicense_mohamed.pdf',  '2026-08-10', 1),
  ('29903030303030',3,'/certificates/syndicatecard_mohamed.pdf','2026-08-10', 1),
  ('29903030303030',4,'/certificates/criminalrecord_mohamed.pdf','2026-08-10', 1);

-- 9. Illnesses
SET IDENTITY_INSERT Illnesses ON;
INSERT INTO Illnesses (Id, Name, Description)
VALUES
  (1,'Diabetes Type 2','Chronic condition affecting insulin usage'),
  (2,'Hypertension',    'High blood pressure'),
  (3,'Asthma',          'Chronic respiratory disease'),
  (4,'Arthritis',       'Joint inflammation');
SET IDENTITY_INSERT Illnesses OFF;
GO

-- 10. PatientIllnesses
INSERT INTO PatientIllnesses (PatientId, IllnessId, DiagnosisDate, Notes)
VALUES
  ('30001010101010',1,'2020-03-15','Controlled with medication'),
  ('30002020202020',2,'2019-07-22','Stage 1 hypertension'),
  ('30003030303030',3,'2018-05-10','Allergy‐induced'),
  ('30004040404040',4,'2021-01-30','Rheumatoid arthritis');

-- 11. Services
SET IDENTITY_INSERT Services ON;
INSERT INTO Services (Id, Name, Description, BasePrice)
VALUES
  (1,'Wound Dressing','Professional wound care and dressing change',150.00),
  (2,'IV Therapy',    'Intravenous medication administration',250.00),
  (3,'Elderly Care',  'Daily assistance for elderly patients',200.00),
  (4,'Post-Op Care',  'Recovery monitoring after surgery',300.00);
SET IDENTITY_INSERT Services OFF;
GO

-- 12. Visits
SET IDENTITY_INSERT Visits ON;
INSERT INTO Visits
(
  Id,
  ScheduledDate,
  ActualVisitDate,
  Status,             -- enum VisitStatus: 0=Pending,1=Confirmed,2=Done,3=InProgress,4=Canceled
  TransportationCost,
  PatientLocation_Lat,
  PatientLocation_Lng,
  NurseLocation_Lat,
  NurseLocation_Lng,
  NurseId,
  PatientId
)
VALUES
  (1,'2023-06-15T10:00:00','2023-06-15T10:15:00',2,50.00,30.049543,31.240123,30.044420,31.235712,'29901010101010','30001010101010'),
  (2,'2023-06-20T14:00:00','2023-06-20T14:30:00',2,60.00,30.047231,31.237654,30.048941,31.238765,'29902020202020','30002020202020'),
  (3,'2023-07-05T09:00:00', NULL           ,0,45.00,30.045678,31.236789,30.042356,31.233421,'29903030303030','30003030303030');
SET IDENTITY_INSERT Visits OFF;
GO

-- 13. VisitServices
INSERT INTO VisitServices (VisitsId, ServicesId)
VALUES
  (1,1),(1,2),
  (2,3),
  (3,4);

-- 14. Payments
SET IDENTITY_INSERT Payments ON;
INSERT INTO Payments
(
  Id,
  PaymentMethod,      -- enum PaymentMethod: 0=Cash,1=CreditCard,2=DebitCard,3=BankTransfer,4=MobilePayment,5=PayPal,6=Fawry
  PaymentDate,
  Status,             -- enum PaymentStatus: 0=Success,1=Failed,2=Pending,3=Refunded,4=Cancelled
  TransactionReference,
  VisitId
)
VALUES
  (1,1,'2023-06-15T11:30:00',0,'PAY-123456',1),
  (2,0,'2023-06-20T15:45:00',0,'PAY-789012',2);
SET IDENTITY_INSERT Payments OFF;
GO

-- 15. Reviewings
SET IDENTITY_INSERT Reviewings ON;
INSERT INTO Reviewings (Id, VisitId, Rating, Comment)
VALUES
  (1,1,5,'Excellent service, very professional'),
  (2,2,4,'Good care but slightly late');
SET IDENTITY_INSERT Reviewings OFF;
GO

-- 16. Notifications
SET IDENTITY_INSERT Notifications ON;
INSERT INTO Notifications
(
  Id,
  Title,
  Body,
  Type,    -- enum NotificationType: 0=Appointment,1=Chat,2=Payment,3=Advertisment,4=System
  IsRead,
  UserId
)
VALUES
  (1,'Appointment Confirmed','Your visit scheduled for June 15 is confirmed',0,1,'30001010101010'),
  (2,'Payment Received',     'Payment of EGP 400.00 for Visit #2 is confirmed',2,0,'30002020202020'),
  (3,'New Message',          'You have 2 unread messages in your chat',1,0,'30003030303030');
SET IDENTITY_INSERT Notifications OFF;
GO

-- 17. ChatReferences
SET IDENTITY_INSERT ChatReferences ON;
INSERT INTO ChatReferences (Id, FirebaseChatId, PatientId, NurseId, IsActive)
VALUES
  (1,'chat_abc123','30001010101010','29901010101010',1),
  (2,'chat_def456','30002020202020','29902020202020',1);
SET IDENTITY_INSERT ChatReferences OFF;
GO

COMMIT TRANSACTION;
