IF OBJECT_ID('ServiceX', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ServiceX (
        Timestmp VARCHAR(20),
        Command VARCHAR(250),
        ForWho VARCHAR(20)
    )
END

IF OBJECT_ID('ServiceXLog', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ServiceXLog (
        Id int PRIMARY KEY IDENTITY(1, 1),
        Command VARCHAR(250),
        RunBy VARCHAR(20),
        RunAt VARCHAR(30),
    CONSTRAINT [UNQ__ServiceXLog__Id] UNIQUE ([Id])
    )
END


IF OBJECT_ID('ServiceXApps', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ServiceXApps (
        ComputerName VARCHAR(250),
        AppLabnet smallint,
        AppEdge smallint,
        AppChrome smallint,
        AppPhotos smallint,
        AppDentDesigner smallint,
        AppDentManager smallint,
        AppModelBuilder smallint,
        AppDentDesktop smallint,
		Ping VARCHAR(20),
		Version VARCHAR(20),
        FriendlyName VARCHAR(250),
        LastCommandTS VARCHAR(20),
    CONSTRAINT [UNQ__ServiceXApps__ComputerName] UNIQUE ([ComputerName])
    )
END

IF OBJECT_ID('ServiceXTime', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ServiceXTime (
        Id int PRIMARY KEY IDENTITY(1, 1),
        SystemTime VARCHAR(20),
    CONSTRAINT [UNQ__ServiceXTime__Id] UNIQUE ([Id])
    )
END