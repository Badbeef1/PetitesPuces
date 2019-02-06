USE [BD6B8_424R_TESTS]
/*USE [BD6B8_424R]*/
/* Table contenant des donnée (20XX)
Catégorie
Client
Vendeur
VendeursClients
TypePoids
TypeLivraison
PoidsLivraison
TaxeProvincial/Fédérale
Produit */

DROP TABLE PPTaxeFederale
DROP TABLE PPTaxeProvinciale
DROP TABLE PPHistoriquePaiements
DROP TABLE PPPoidsLivraisons
DROP TABLE PPTypesPoids
DROP TABLE PPDetailsCommandes
DROP TABLE PPCommandes
DROP TABLE PPTypesLivraison
DROP TABLE PPArticlesEnPanier
DROP TABLE PPVendeursClients
DROP TABLE PPEvaluations
DROP TABLE PPProduits
DROP TABLE PPCategories
DROP TABLE HistoCommandes
DROP TABLE HistoDetailsCommandes
DROP TABLE PPVendeurs
DROP TABLE PPClients
DROP TABLE PPGestionnaire
DROP TABLE PPDestinataires
DROP TABLE PPMessages
DROP TABLE PPLieu

GO
/****** Object:  Table [dbo].[PPArticlesEnPanier]    Script Date: 2019-01-24 12:34:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PPArticlesEnPanier](
	[NoPanier] [bigint] NOT NULL,
	[NoClient] [bigint] NULL,
	[NoVendeur] [bigint] NULL,
	[NoProduit] [bigint] NULL,
	[DateCreation] [smalldatetime] NULL,
	[NbItems] [smallint] NULL,
 CONSTRAINT [PK_PPArticlesEnPanier] PRIMARY KEY CLUSTERED 
(
	[NoPanier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PPCategories]    Script Date: 2019-01-24 12:34:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PPCategories](
	[NoCategorie] [int] NOT NULL,
	[Description] [varchar](50) NULL,
	[Details] [varchar](max) NULL,
 CONSTRAINT [PK_PPCategories] PRIMARY KEY CLUSTERED 
(
	[NoCategorie] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PPClients]    Script Date: 2019-01-24 12:34:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PPClients](
	[NoClient] [bigint] NOT NULL,
	[AdresseEmail] [varchar](100) NULL,
	[MotDePasse] [varchar](50) NULL,
	[Nom] [varchar](50) NULL,
	[Prenom] [varchar](50) NULL,
	[Rue] [varchar](50) NULL,
	[Ville] [varchar](50) NULL,
	[Province] [char](2) NULL,
	[CodePostal] [varchar](7) NULL,
	[Pays] [varchar](10) NULL,
	[Tel1] [varchar](20) NULL,
	[Tel2] [varchar](20) NULL,
	[DateCreation] [smalldatetime] NULL,
	[DateMAJ] [smalldatetime] NULL,
	[NbConnexions] [smallint] NULL,
	[DateDerniereConnexion] [smalldatetime] NULL,
	[Statut] [smallint] NULL,
 CONSTRAINT [PK_PPClients] PRIMARY KEY CLUSTERED 
(
	[NoClient] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PPCommandes]    Script Date: 2019-01-24 12:34:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PPCommandes](
	[NoCommande] [bigint] NOT NULL,
	[NoClient] [bigint] NULL,
	[NoVendeur] [bigint] NULL,
	[DateCommande] [smalldatetime] NULL,
	[CoutLivraison] [smallmoney] NULL,
	[TypeLivraison] [smallint] NULL,
	[MontantTotAvantTaxes] [smallmoney] NULL,
	[TPS] [smallmoney] NULL,
	[TVQ] [smallmoney] NULL,
	[PoidsTotal] [numeric](8, 1) NULL,
	[Statut] [char](1) NULL,
	[NoAutorisation] [varchar](50) NULL,
 CONSTRAINT [PK_PPCommandes] PRIMARY KEY CLUSTERED 
(
	[NoCommande] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PPDetailsCommandes]    Script Date: 2019-01-24 12:34:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PPDetailsCommandes](
	[NoDetailCommandes] [bigint] NOT NULL,
	[NoCommande] [bigint] NULL,
	[NoProduit] [bigint] NULL,
	[PrixVente] [smallmoney] NULL,
	[Quantité] [smallint] NULL,
 CONSTRAINT [PK_PPDetailsCommandes] PRIMARY KEY CLUSTERED 
(
	[NoDetailCommandes] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PPHistoriquePaiements]    Script Date: 2019-01-24 12:34:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PPHistoriquePaiements](
	[NoHistorique] [bigint] NOT NULL,
	[MontantVenteAvantLivraison] [smallmoney] NULL,
	[NoVendeur] [bigint] NULL,
	[NoClient] [bigint] NULL,
	[NoCommande] [bigint] NULL,
	[DateVente] [smalldatetime] NULL,
	[NoAutorisation] [varchar](50) NULL,
	[FraisLesi] [smallmoney] NULL,
	[Redevance] [smallmoney] NULL,
	[FraisLivraison] [smallmoney] NULL,
	[FraisTPS] [smallmoney] NULL,
	[FraisTVQ] [smallmoney] NULL,
 CONSTRAINT [PK_PPHistoriquePaiements] PRIMARY KEY CLUSTERED 
(
	[NoHistorique] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PPPoidsLivraisons]    Script Date: 2019-01-24 12:34:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PPPoidsLivraisons](
	[CodeLivraison] [smallint] NOT NULL,
	[CodePoids] [smallint] NOT NULL,
	[Tarif] [smallmoney] NULL,
 CONSTRAINT [PK_PPPoidsLivraisons] PRIMARY KEY CLUSTERED 
(
	[CodeLivraison] ASC,
	[CodePoids] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PPProduits]    Script Date: 2019-01-24 12:34:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PPProduits](
	[NoProduit] [bigint] NOT NULL,
	[NoVendeur] [bigint] NULL,
	[NoCategorie] [int] NULL,
	[Nom] [varchar](50) NULL,
	[Description] [varchar](max) NULL,
	[Photo] [varchar](50) NULL,
	[PrixDemande] [smallmoney] NULL,
	[NombreItems] [smallint] NULL,
	[Disponibilité] [bit] NULL,
	[DateVente] [smalldatetime] NULL,
	[PrixVente] [smallmoney] NULL,
	[Poids] [numeric](8, 1) NULL,
	[DateCreation] [smalldatetime] NULL,
	[DateMAJ] [smalldatetime] NULL,
 CONSTRAINT [PK_PPProduits] PRIMARY KEY CLUSTERED 
(
	[NoProduit] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PPTaxeFederale]    Script Date: 2019-01-24 12:34:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PPTaxeFederale](
	[NoTPS] [tinyint] NOT NULL,
	[DateEffectiveTPS] [smalldatetime] NULL,
	[TauxTPS] [numeric](4, 2) NULL,
 CONSTRAINT [PK_PPTaxeFederale] PRIMARY KEY CLUSTERED 
(
	[NoTPS] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PPTaxeProvinciale]    Script Date: 2019-01-24 12:34:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PPTaxeProvinciale](
	[NoTVQ] [tinyint] NOT NULL,
	[DateEffectiveTVQ] [smalldatetime] NULL,
	[TauxTVQ] [numeric](5, 3) NULL,
 CONSTRAINT [PK_PPTaxeProvinciale] PRIMARY KEY CLUSTERED 
(
	[NoTVQ] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PPTypesLivraison]    Script Date: 2019-01-24 12:34:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PPTypesLivraison](
	[CodeLivraison] [smallint] NOT NULL,
	[Description] [varchar](50) NULL,
 CONSTRAINT [PK_PPTypesLivraison] PRIMARY KEY CLUSTERED 
(
	[CodeLivraison] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PPTypesPoids]    Script Date: 2019-01-24 12:34:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PPTypesPoids](
	[CodePoids] [smallint] NOT NULL,
	[PoidsMin] [numeric](8, 1) NULL,
	[PoidsMax] [numeric](8, 1) NULL,
 CONSTRAINT [PK_PPTypesPoids] PRIMARY KEY CLUSTERED 
(
	[CodePoids] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PPVendeurs]    Script Date: 2019-01-24 12:34:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PPVendeurs](
	[NoVendeur] [bigint] NOT NULL,
	[NomAffaires] [varchar](50) NULL,
	[Nom] [varchar](50) NULL,
	[Prenom] [varchar](50) NULL,
	[Rue] [varchar](50) NULL,
	[Ville] [varchar](50) NULL,
	[Province] [char](2) NULL,
	[CodePostal] [varchar](7) NULL,
	[Pays] [varchar](10) NULL,
	[Tel1] [varchar](20) NULL,
	[Tel2] [varchar](20) NULL,
	[AdresseEmail] [varchar](100) NULL,
	[MotDePasse] [varchar](50) NULL,
	[PoidsMaxLivraison] [int] NULL,
	[LivraisonGratuite] [smallmoney] NULL,
	[Taxes] [bit] NULL,
	[Pourcentage] [numeric](4, 2) NULL,
	[Configuration] [varchar](512) NULL,
	[DateCreation] [smalldatetime] NULL,
	[DateMAJ] [smalldatetime] NULL,
	[Statut] [smallint] NULL,
 CONSTRAINT [PK_PPVendeurs] PRIMARY KEY CLUSTERED 
(
	[NoVendeur] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PPVendeursClients]    Script Date: 2019-01-24 12:34:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PPVendeursClients](
	[NoVendeur] [bigint] NOT NULL,
	[NoClient] [bigint] NOT NULL,
	[DateVisite] [smalldatetime] NOT NULL,
 CONSTRAINT [PK_PPVendeursClients] PRIMARY KEY CLUSTERED 
(
	[NoVendeur] ASC,
	[NoClient] ASC,
	[DateVisite] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[PPArticlesEnPanier]  WITH CHECK ADD  CONSTRAINT [FK_PPArticlesEnPanier_PPClients] FOREIGN KEY([NoClient])
REFERENCES [dbo].[PPClients] ([NoClient])
GO
ALTER TABLE [dbo].[PPArticlesEnPanier] CHECK CONSTRAINT [FK_PPArticlesEnPanier_PPClients]
GO
ALTER TABLE [dbo].[PPArticlesEnPanier]  WITH CHECK ADD  CONSTRAINT [FK_PPArticlesEnPanier_PPProduits] FOREIGN KEY([NoProduit])
REFERENCES [dbo].[PPProduits] ([NoProduit])
GO
ALTER TABLE [dbo].[PPArticlesEnPanier] CHECK CONSTRAINT [FK_PPArticlesEnPanier_PPProduits]
GO
ALTER TABLE [dbo].[PPArticlesEnPanier]  WITH CHECK ADD  CONSTRAINT [FK_PPArticlesEnPanier_PPVendeurs] FOREIGN KEY([NoVendeur])
REFERENCES [dbo].[PPVendeurs] ([NoVendeur])
GO
ALTER TABLE [dbo].[PPArticlesEnPanier] CHECK CONSTRAINT [FK_PPArticlesEnPanier_PPVendeurs]
GO
ALTER TABLE [dbo].[PPCommandes]  WITH CHECK ADD  CONSTRAINT [FK_PPCommandes_PPClients] FOREIGN KEY([NoClient])
REFERENCES [dbo].[PPClients] ([NoClient])
GO
ALTER TABLE [dbo].[PPCommandes] CHECK CONSTRAINT [FK_PPCommandes_PPClients]
GO
ALTER TABLE [dbo].[PPCommandes]  WITH CHECK ADD  CONSTRAINT [FK_PPCommandes_PPTypesLivraison] FOREIGN KEY([TypeLivraison])
REFERENCES [dbo].[PPTypesLivraison] ([CodeLivraison])
GO
ALTER TABLE [dbo].[PPCommandes] CHECK CONSTRAINT [FK_PPCommandes_PPTypesLivraison]
GO
ALTER TABLE [dbo].[PPCommandes]  WITH CHECK ADD  CONSTRAINT [FK_PPCommandes_PPVendeurs] FOREIGN KEY([NoVendeur])
REFERENCES [dbo].[PPVendeurs] ([NoVendeur])
GO
ALTER TABLE [dbo].[PPCommandes] CHECK CONSTRAINT [FK_PPCommandes_PPVendeurs]
GO
ALTER TABLE [dbo].[PPDetailsCommandes]  WITH CHECK ADD  CONSTRAINT [FK_PPDetailsCommandes_PPCommandes] FOREIGN KEY([NoCommande])
REFERENCES [dbo].[PPCommandes] ([NoCommande])
GO
ALTER TABLE [dbo].[PPDetailsCommandes] CHECK CONSTRAINT [FK_PPDetailsCommandes_PPCommandes]
GO
ALTER TABLE [dbo].[PPDetailsCommandes]  WITH CHECK ADD  CONSTRAINT [FK_PPDetailsCommandes_PPProduits] FOREIGN KEY([NoProduit])
REFERENCES [dbo].[PPProduits] ([NoProduit])
GO
ALTER TABLE [dbo].[PPDetailsCommandes] CHECK CONSTRAINT [FK_PPDetailsCommandes_PPProduits]
GO
ALTER TABLE [dbo].[PPPoidsLivraisons]  WITH CHECK ADD  CONSTRAINT [FK_PPPoidsLivraisons_PPTypesLivraison] FOREIGN KEY([CodeLivraison])
REFERENCES [dbo].[PPTypesLivraison] ([CodeLivraison])
GO
ALTER TABLE [dbo].[PPPoidsLivraisons] CHECK CONSTRAINT [FK_PPPoidsLivraisons_PPTypesLivraison]
GO
ALTER TABLE [dbo].[PPPoidsLivraisons]  WITH CHECK ADD  CONSTRAINT [FK_PPPoidsLivraisons_PPTypesPoids] FOREIGN KEY([CodePoids])
REFERENCES [dbo].[PPTypesPoids] ([CodePoids])
GO
ALTER TABLE [dbo].[PPPoidsLivraisons] CHECK CONSTRAINT [FK_PPPoidsLivraisons_PPTypesPoids]
GO
ALTER TABLE [dbo].[PPProduits]  WITH CHECK ADD  CONSTRAINT [FK_PPProduits_PPCategories] FOREIGN KEY([NoCategorie])
REFERENCES [dbo].[PPCategories] ([NoCategorie])
GO
ALTER TABLE [dbo].[PPProduits] CHECK CONSTRAINT [FK_PPProduits_PPCategories]
GO
ALTER TABLE [dbo].[PPProduits]  WITH CHECK ADD  CONSTRAINT [FK_PPProduits_PPVendeurs1] FOREIGN KEY([NoVendeur])
REFERENCES [dbo].[PPVendeurs] ([NoVendeur])
GO
ALTER TABLE [dbo].[PPProduits] CHECK CONSTRAINT [FK_PPProduits_PPVendeurs1]
GO
ALTER TABLE [dbo].[PPVendeursClients]  WITH CHECK ADD  CONSTRAINT [FK_PPVendeursClients_PPClients] FOREIGN KEY([NoClient])
REFERENCES [dbo].[PPClients] ([NoClient])
GO
ALTER TABLE [dbo].[PPVendeursClients] CHECK CONSTRAINT [FK_PPVendeursClients_PPClients]
GO
ALTER TABLE [dbo].[PPVendeursClients]  WITH CHECK ADD  CONSTRAINT [FK_PPVendeursClients_PPVendeurs] FOREIGN KEY([NoVendeur])
REFERENCES [dbo].[PPVendeurs] ([NoVendeur])
GO
ALTER TABLE [dbo].[PPVendeursClients] CHECK CONSTRAINT [FK_PPVendeursClients_PPVendeurs]
GO

CREATE TABLE PPLieu(
	NoLieu smallint,
	Description nvarchar(50),
	PRIMARY KEY(NoLieu)
	);
CREATE TABLE PPMessages(
	NoMsg int,
	NoExpediteur int,
	DescMsg nvarchar(MAX),
	FichierJoint sql_variant,
	Lieu smallint,
	dateEnvoi smalldatetime,
	objet nvarchar(50),
	PRIMARY KEY(NoMsg),
	FOREIGN KEY(Lieu) REFERENCES PPLieu(NoLieu)
);

CREATE TABLE PPDestinataires (
	NoMsg int,
	NoDestinataire int,
	EtatLu smallint,
	Lieu smallint,
	CONSTRAINT PK_Destinataire PRIMARY KEY (NoMsg,NoDestinataire),
	CONSTRAINT FK_NoMsg FOREIGN KEY (NoMsg) REFERENCES PPMessages (NoMsg),
	CONSTRAINT FK_Lieu FOREIGN KEY(Lieu) REFERENCES PPLieu(NoLieu)
	);

CREATE TABLE PPGestionnaire(
	AdresseEmail varchar(100) NOT NULL,
	MotDePasse varchar(50) NOT NULL,
	PRIMARY KEY(AdresseEmail)
);

CREATE TABLE PPEvaluations(
	NoClient bigint NOT NULL,
	NoProduit bigint NOT NULL,
	Cote numeric(1,0),
	Commentaire varchar(150),
	DateMAJ smalldatetime,
	DateCreation smalldatetime
	);