
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 06/30/2017 16:48:15
-- Generated from EDMX file: F:\Code\DecisionSupport\DSWeb\Models\DS.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [dotnet_erp257sp1_chengyu_zqdy_branch6];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[DSTree]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DSTree];
GO
IF OBJECT_ID(N'[dbo].[DSTreeCEMap]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DSTreeCEMap];
GO
IF OBJECT_ID(N'[dbo].[DSTreeModel]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DSTreeModel];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'DSTree'
CREATE TABLE [dbo].[DSTree] (
    [DSTreeGUID] uniqueidentifier  NOT NULL,
    [ModGUID] uniqueidentifier  NULL,
    [ID] varchar(100)  NULL,
    [PID] varchar(100)  NULL,
    [FactorName] varchar(100)  NULL,
    [FactorNameCn] varchar(100)  NULL,
    [Operator] varchar(50)  NULL,
    [OperatorCn] varchar(50)  NULL,
    [FactorValue] varchar(max)  NULL,
    [FactorValueCn] varchar(max)  NULL,
    [Describe] varchar(max)  NULL,
    [DescribeCn] varchar(max)  NULL,
    [Result] varchar(500)  NULL,
    [ResultCn] varchar(500)  NULL,
    [CoverCount] int  NULL,
    [ErrorCount] int  NULL
);
GO

-- Creating table 'DSTreeCEMap'
CREATE TABLE [dbo].[DSTreeCEMap] (
    [CEMapGUID] uniqueidentifier  NOT NULL,
    [ModGUID] uniqueidentifier  NULL,
    [ECellName] varchar(50)  NULL,
    [CCellName] varchar(50)  NULL,
    [IsResultFactor] bit  NULL
);
GO

-- Creating table 'DSTreeModel'
CREATE TABLE [dbo].[DSTreeModel] (
    [ModGUID] uniqueidentifier  NOT NULL,
    [ModName] varchar(50)  NULL,
    [ModGenerateTime] datetime  NULL,
    [ModDataSource] varchar(max)  NULL,
    [ModRemark] varchar(max)  NULL,
    [IsFileDataSource] bit  NULL,
    [ModServer] varchar(50)  NULL,
    [ModDataBase] varchar(50)  NULL,
    [ModUid] varchar(50)  NULL,
    [ModPassword] varchar(50)  NULL,
    [ModCycleType] varchar(50)  NULL,
    [ModCycleWD] int  NULL,
    [ModCycleDate] int  NULL,
    [ModStatus] varchar(50)  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [DSTreeGUID] in table 'DSTree'
ALTER TABLE [dbo].[DSTree]
ADD CONSTRAINT [PK_DSTree]
    PRIMARY KEY CLUSTERED ([DSTreeGUID] ASC);
GO

-- Creating primary key on [CEMapGUID] in table 'DSTreeCEMap'
ALTER TABLE [dbo].[DSTreeCEMap]
ADD CONSTRAINT [PK_DSTreeCEMap]
    PRIMARY KEY CLUSTERED ([CEMapGUID] ASC);
GO

-- Creating primary key on [ModGUID] in table 'DSTreeModel'
ALTER TABLE [dbo].[DSTreeModel]
ADD CONSTRAINT [PK_DSTreeModel]
    PRIMARY KEY CLUSTERED ([ModGUID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------