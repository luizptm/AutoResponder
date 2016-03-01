/**********************************/
/*** TABLE [BR_AutoResponder_Sending] ***/
/**********************************/
CREATE TABLE [dbo].[BR_AutoResponder_Sending](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[TemplateId] [int] NOT NULL,
	[SentDate] [datetime] NULL,
	[Unsubscribe] [bit] NULL,
	[OpenMail] [bit] NULL,
	[Bounce] [bit] NULL,
	[Click] [bit] NULL,
	[CREATION_DATE] [datetime] NOT NULL,
 CONSTRAINT [PK_AutoResponder_Sending] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

/**********************************/
/*** TABLE [BR_AutoResponder_SendingList] ***/
/**********************************/
CREATE TABLE [dbo].[BR_AutoResponder_SendingList](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[TagId] [int] NOT NULL,
	[Interval] [int] NOT NULL,
	[CREATION_DATE] [datetime] NOT NULL,
 CONSTRAINT [PK_AutoResponder_SendingList] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

/**********************************/
/*** TABLE [BR_AutoResponder_Tag] ***/
/**********************************/
CREATE TABLE [dbo].[BR_AutoResponder_Tag](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[CREATION_DATE] [datetime] NOT NULL,
 CONSTRAINT [PK_AutoResponder_TAG] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

/**********************************/
/*** TABLE [BR_AutoResponder_Template] ***/
/**********************************/
CREATE TABLE [dbo].[BR_AutoResponder_Template](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SendingListId] [int] NOT NULL,
	[Sequence] [int] NOT NULL,
	[Interval] [int] NULL,
	[Sender] [nvarchar](max) NOT NULL,
	[Subject] [nvarchar](max) NOT NULL,
	[Body] [text] NOT NULL,
	[CREATION_DATE] [datetime] NOT NULL,
 CONSTRAINT [PK_AutoResponder_Template] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

/**********************************/
/*** TABLE [BR_Users] ***/
/**********************************/
CREATE TABLE [dbo].[BR_Users](
	[idUser] [int] IDENTITY(1,1) NOT NULL,
	[firstName] [varchar](500) NULL,
	[middleName] [varchar](500) NULL,
	[lastName] [varchar](500) NULL,
	[gender] [varchar](15) NULL,
	[birthday] [varchar](50) NULL,
	[email] [varchar](250) NULL,
	[address] [varchar](500) NULL,
	[city] [varchar](200) NULL,
	[state] [char](2) NULL,
	[UPDATE_TIME] [datetime] NULL,
	[CREATION_TIME] [datetime] NULL,
	[zipcode] [nchar](9) NULL,
	[cpf] [varchar](50) NULL,
	[hometown] [varchar](150) NULL,
	[navigateURL] [varchar](150) NULL,
	[verified] [bit] NULL,
	[relationshipStatus] [varchar](50) NULL,
	[religion] [varchar](max) NULL,
	[politicalView] [varchar](max) NULL,
	[biography] [varchar](max) NULL,
	[languages] [varchar](max) NULL,
	[timezone] [float] NULL,
	[location] [varchar](150) NULL,
	[username] [varchar](50) NULL,
	[name] [varchar](450) NULL,
	[userID] [varchar](50) NULL,
	[rg] [nchar](50) NULL,
	[telephone] [varchar](14) NULL,
	[tags] [varchar](max) NULL,
 CONSTRAINT [PK_BR_Users] PRIMARY KEY CLUSTERED 
(
	[idUser] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

/**********************************/
/*** TABLE [BR_AutoResponder_UserEntry] ***/
/**********************************/
CREATE TABLE [dbo].[BR_AutoResponder_UserEntry](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[SendingListId] [int] NOT NULL,
	[EntryDate] [datetime] NOT NULL,
	[Optin] [bit] NOT NULL,
	[CREATION_DATE] [datetime] NOT NULL,
 CONSTRAINT [PK_AutoResponder_UserEntry] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

/**********************************/
/*** TABLE [BR_AutoResponder_User_Tag] ***/
/**********************************/
CREATE TABLE [dbo].[BR_AutoResponder_User_Tag](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[TagId] [int] NOT NULL,
	[CREATION_DATE] [datetime] NULL,
 CONSTRAINT [PK_BR_AutoResponder_User_Tag] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

/**************************/
/*** DEFAULT CONSTRAINTS **/
/**************************/
ALTER TABLE [dbo].[BR_AutoResponder_Sending] ADD  CONSTRAINT [DF_BR_AutoResponder_Sending_CREATION_DATE]  DEFAULT (getdate()) FOR [CREATION_DATE]
ALTER TABLE [dbo].[BR_AutoResponder_SendingList] ADD  CONSTRAINT [DF_BR_AutoResponder_SendingList_CREATION_DATE]  DEFAULT (getdate()) FOR [CREATION_DATE]
ALTER TABLE [dbo].[BR_AutoResponder_Tag] ADD  CONSTRAINT [DF_BR_AutoResponder_Tag_CREATION_DATE]  DEFAULT (getdate()) FOR [CREATION_DATE]
ALTER TABLE [dbo].[BR_AutoResponder_Template] ADD  CONSTRAINT [DF_BR_AutoResponder_Template_CREATION_DATE]  DEFAULT (getdate()) FOR [CREATION_DATE]
ALTER TABLE [dbo].[BR_AutoResponder_UserEntry] ADD  CONSTRAINT [DF_BR_AutoResponder_UserEntry_CREATION_DATE]  DEFAULT (getdate()) FOR [CREATION_DATE]

/**************************/
/*** FOREIGN CONSTRAINTS **/
/**************************/
ALTER TABLE [dbo].[BR_AutoResponder_Sending]  WITH CHECK ADD  CONSTRAINT [FK_BR_AutoResponder_Sending_BR_AutoResponder_Template] FOREIGN KEY([TemplateId])
	REFERENCES [dbo].[BR_AutoResponder_Template] ([Id])
	ON DELETE CASCADE
ALTER TABLE [dbo].[BR_AutoResponder_Sending]  WITH CHECK ADD  CONSTRAINT [FK_BR_AutoResponder_Sending_BR_Users] FOREIGN KEY([UserId])
	REFERENCES [dbo].[BR_Users] ([idUser])
	ON DELETE CASCADE
ALTER TABLE [dbo].[BR_AutoResponder_SendingList]  WITH CHECK ADD  CONSTRAINT [FK_BR_AutoResponder_SendingList_BR_AutoResponder_Tag] FOREIGN KEY([TagId])
	REFERENCES [dbo].[BR_AutoResponder_Tag] ([Id])
	ON DELETE CASCADE
ALTER TABLE [dbo].[BR_AutoResponder_Template]  WITH CHECK ADD  CONSTRAINT [FK_AutoResponder_Template_AutoResponder_SendingList] FOREIGN KEY([SendingListId])
	REFERENCES [dbo].[BR_AutoResponder_SendingList] ([Id])
	ON DELETE CASCADE
ALTER TABLE [dbo].[BR_AutoResponder_User_Tag]  WITH CHECK ADD  CONSTRAINT [FK_BR_AutoResponder_User_Tag_BR_AutoResponder_Tag] FOREIGN KEY([TagId])
	REFERENCES [dbo].[BR_AutoResponder_Tag] ([Id])
	ON DELETE CASCADE
ALTER TABLE [dbo].[BR_AutoResponder_User_Tag]  WITH CHECK ADD  CONSTRAINT [FK_BR_AutoResponder_User_Tag_BR_Users] FOREIGN KEY([UserId])
	REFERENCES [dbo].[BR_Users] ([idUser])
	ON DELETE CASCADE
ALTER TABLE [dbo].[BR_AutoResponder_UserEntry]  WITH CHECK ADD  CONSTRAINT [FK_AutoResponder_UserEntry_AutoResponder_SendingList] FOREIGN KEY([SendingListId])
	REFERENCES [dbo].[BR_AutoResponder_SendingList] ([Id])
	ON DELETE CASCADE
ALTER TABLE [dbo].[BR_AutoResponder_UserEntry]  WITH CHECK ADD  CONSTRAINT [FK_BR_AutoResponder_UserEntry_BR_Users] FOREIGN KEY([UserId])
	REFERENCES [dbo].[BR_Users] ([idUser])
	ON DELETE CASCADE

/**************************/
/*** CHECK CONSTRAINTS **/
/**************************/
ALTER TABLE [dbo].[BR_AutoResponder_Sending] CHECK CONSTRAINT [FK_BR_AutoResponder_Sending_BR_AutoResponder_Template]
ALTER TABLE [dbo].[BR_AutoResponder_SendingList] CHECK CONSTRAINT [FK_BR_AutoResponder_SendingList_BR_AutoResponder_Tag]
ALTER TABLE [dbo].[BR_AutoResponder_Template] CHECK CONSTRAINT [FK_AutoResponder_Template_AutoResponder_SendingList]
ALTER TABLE [dbo].[BR_AutoResponder_User_Tag] CHECK CONSTRAINT [FK_BR_AutoResponder_User_Tag_BR_AutoResponder_Tag]
ALTER TABLE [dbo].[BR_AutoResponder_User_Tag] CHECK CONSTRAINT [FK_BR_AutoResponder_User_Tag_BR_Users]
ALTER TABLE [dbo].[BR_AutoResponder_UserEntry] CHECK CONSTRAINT [FK_AutoResponder_UserEntry_AutoResponder_SendingList]
