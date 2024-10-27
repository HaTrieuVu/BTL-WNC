
-------------------------------------------------- Here

-- 1. Bảng Companies: Quản lý thông tin doanh nghiệp (tenant).
CREATE TABLE Companies (
    CompanyId INT PRIMARY KEY IDENTITY,
    CompanyName NVARCHAR(100) NOT NULL,
    Domain NVARCHAR(100) NOT NULL,  -- Tên miền doanh nghiệp
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);

-- 2. Bảng Users: Quản lý thông tin người dùng.
CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
	Phone NVARCHAR(15) NULL,
    Role NVARCHAR(50) CHECK (Role IN ('Admin', 'Manager', 'Employee')) NOT NULL,
    CompanyId INT FOREIGN KEY REFERENCES Companies(CompanyId),
	RoleId INT FOREIGN KEY REFERENCES Roles(RoleId)
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsActive BIT NOT NULL DEFAULT 1
);

-- Tạo bảng Roles để lưu các vai trò của hệ thống
CREATE TABLE Roles (
    RoleId INT PRIMARY KEY IDENTITY,
    RoleName NVARCHAR(50) NOT NULL UNIQUE,  -- Ví dụ: 'Admin', 'Manager', 'Employee'
	Description NVARCHAR(50)
);


-- Tạo bảng UserRoles để ánh xạ giữa người dùng và vai trò của họ
CREATE TABLE UserRoles (
    UserId INT,
    RoleId INT,
    CONSTRAINT FK_UserRoles_Users FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
    CONSTRAINT FK_UserRoles_Roles FOREIGN KEY (RoleId) REFERENCES Roles(RoleId) ON DELETE CASCADE,
    PRIMARY KEY (UserId, RoleId)  -- Đảm bảo mỗi người dùng có thể có nhiều vai trò nhưng không được trùng vai trò
);


-- 3. Bảng CompanySettings: Lưu trữ các thiết lập tùy chỉnh cho từng doanh nghiệp.
CREATE TABLE CompanySettings (
    CompanyId INT PRIMARY KEY FOREIGN KEY REFERENCES Companies(CompanyId),
    Theme NVARCHAR(50),  -- Giao diện tùy chỉnh
    Logo NVARCHAR(255)   -- Đường dẫn logo
);

-- 4. Bảng SubscriptionPlans: Quản lý các gói dịch vụ.
CREATE TABLE SubscriptionPlans (
    PlanId INT PRIMARY KEY IDENTITY,
    PlanName NVARCHAR(50) NOT NULL,
    MaxUsers INT,
    MaxStorage INT,  -- Dung lượng lưu trữ tối đa (MB)
    Price DECIMAL(10, 2) NOT NULL  -- Giá gói dịch vụ
);

-- 5. Bảng CompanySubscriptions: Quản lý gói dịch vụ của mỗi doanh nghiệp.
CREATE TABLE CompanySubscriptions (
    CompanyId INT PRIMARY KEY FOREIGN KEY REFERENCES Companies(CompanyId),
    PlanId INT NOT NULL FOREIGN KEY REFERENCES SubscriptionPlans(PlanId),
    StartDate DATE NOT NULL,
    EndDate DATE
);

-- 6. Bảng Invoices: Lưu trữ hóa đơn của các doanh nghiệp.
CREATE TABLE Invoices (
    InvoiceId INT PRIMARY KEY IDENTITY,
    CompanyId INT NOT NULL FOREIGN KEY REFERENCES Companies(CompanyId),
    Amount DECIMAL(10, 2) NOT NULL,
    PaymentDate DATETIME NOT NULL DEFAULT GETDATE(),
    Status NVARCHAR(50) CHECK (Status IN ('Paid', 'Pending', 'Failed'))
);

-- 7. Bảng Projects: Quản lý các dự án.
CREATE TABLE Projects (
    ProjectId INT PRIMARY KEY IDENTITY,
    ProjectName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255),
    StartDate DATE,
    EndDate DATE,
    CompanyId INT NOT NULL FOREIGN KEY REFERENCES Companies(CompanyId),  -- Liên kết với doanh nghiệp
    CreatedBy INT NOT NULL FOREIGN KEY REFERENCES Users(UserId),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    Status NVARCHAR(50) CHECK (Status IN ('Planned', 'InProgress', 'Completed', 'Cancelled')) NOT NULL
);

-- 8. Bảng Tasks: Quản lý các nhiệm vụ trong dự án.
CREATE TABLE Tasks (
    TaskId INT PRIMARY KEY IDENTITY,
    TaskName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255),
    ProjectId INT NOT NULL FOREIGN KEY REFERENCES Projects(ProjectId),
    AssignedTo INT FOREIGN KEY REFERENCES Users(UserId),
    Priority NVARCHAR(50) CHECK (Priority IN ('Low', 'Medium', 'High')) NOT NULL,
    DueDate DATE,
    Status NVARCHAR(50) CHECK (Status IN ('NotStarted', 'InProgress', 'Completed')) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);

-- 9. Bảng Messages: Quản lý tin nhắn giữa người dùng.
CREATE TABLE Messages (
    MessageId INT PRIMARY KEY IDENTITY,
    SenderId INT NOT NULL FOREIGN KEY REFERENCES Users(UserId),
    ReceiverId INT FOREIGN KEY REFERENCES Users(UserId),  -- NULL nếu là tin nhắn nhóm
    GroupId INT,  -- Nếu là tin nhắn nhóm
	MessageType VARCHAR(50), --MessageType để quản lý loại tin nhắn (text, file, image, etc.)
    Content NVARCHAR(MAX) NOT NULL,
    SentAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsRead BIT NOT NULL DEFAULT 0
);

-- 10. Bảng Groups: Quản lý các nhóm nhắn tin.
CREATE TABLE Groups (
    GroupId INT PRIMARY KEY IDENTITY,
    GroupName NVARCHAR(100) NOT NULL,
    CreatedBy INT NOT NULL FOREIGN KEY REFERENCES Users(UserId),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);

-- 11. Bảng GroupMembers: Quản lý thành viên trong nhóm nhắn tin.
CREATE TABLE GroupMembers (
    GroupId INT NOT NULL FOREIGN KEY REFERENCES Groups(GroupId),
    UserId INT NOT NULL FOREIGN KEY REFERENCES Users(UserId),
    JoinedAt DATETIME NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY (GroupId, UserId)
);

-- 12. Bảng Files: Quản lý các tệp tin được chia sẻ trong dự án hoặc tin nhắn.
CREATE TABLE Files (
    FileId INT PRIMARY KEY IDENTITY,
    FileName NVARCHAR(255) NOT NULL,
    FilePath NVARCHAR(255) NOT NULL,  -- Đường dẫn lưu trữ tệp
    UploadedBy INT NOT NULL FOREIGN KEY REFERENCES Users(UserId),
    UploadedAt DATETIME NOT NULL DEFAULT GETDATE(),
    ProjectId INT FOREIGN KEY REFERENCES Projects(ProjectId),  -- NULL nếu không liên quan dự án
    MessageId INT FOREIGN KEY REFERENCES Messages(MessageId),  -- NULL nếu không liên quan tin nhắn
	FileType VARCHAR(50)  -- Quản lý loại tệp (PDF, Word, Image, etc.)
);

-- 13. Bảng Notifications: Quản lý các thông báo gửi đến người dùng.
CREATE TABLE Notifications (
    NotificationId INT PRIMARY KEY IDENTITY,
    UserId INT NOT NULL FOREIGN KEY REFERENCES Users(UserId),
    Message NVARCHAR(255) NOT NULL,
    IsRead BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);

-- 14. Bảng UserSessions: Quản lý các phiên đăng nhập của người dùng.
CREATE TABLE UserSessions (
    SessionId INT PRIMARY KEY IDENTITY,
    UserId INT NOT NULL FOREIGN KEY REFERENCES Users(UserId),
    LoginTime DATETIME NOT NULL DEFAULT GETDATE(),
    LogoutTime DATETIME,
    IsActive BIT NOT NULL DEFAULT 1
);




