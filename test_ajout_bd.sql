use BD6B8_424R
use BD6B8_424R_TESTS

select * from PPClients
select * from PPVendeurs

select * from PPCategories
select * from PPProduits
select * from PPArticlesEnPanier
SELECT * FROM PPTypesLivraison
select * from PPCommandes


--Ajouter un vendeur
insert into PPVendeurs values(1,'Childish', 'Blanchet-Plante', 'Kevin','Notre-Dame', 'Montréal','QC','J7W8H5', 'Canada','514-453-8546',null,'kev@gmail.com','Password1',10,20,1,10,null,GETDATE(),GETDATE(),1)

/* Rajouter des paniers aux utilisateurs*/
INSERT INTO PPArticlesEnPanier
VALUES (1, 10000, 10, 1000010, GETDATE(), 1);

INSERT INTO PPArticlesEnPanier
VALUES (2, 10000, 10, 1000020, GETDATE(), 1);

INSERT INTO PPArticlesEnPanier
VALUES (3, 10000, 10, 1000030, GETDATE(), 1);

INSERT INTO PPArticlesEnPanier
VALUES (4, 10000, 20, 2000010, GETDATE(), 1);

INSERT INTO PPArticlesEnPanier
VALUES (5, 10000, 20, 2000020, GETDATE(), 1);

INSERT INTO PPArticlesEnPanier
VALUES (6, 10000, 20, 2000030, GETDATE(), 1);

INSERT INTO PPArticlesEnPanier
VALUES (7, 10000, 20, 2000040, GETDATE(), 1);

INSERT INTO PPArticlesEnPanier
VALUES (8,10100,10,1000020,GETDATE(),1)

delete PPVendeurs


--Ajouter des produits
insert into PPProduits 
values(2000050,20,3,'Apple Watch série 3','(GPS + cellulaire) 38 mm boîtier aluminium doré + bracelet sport rose des sables','2000050.jpg',430.24,3,1,NULL,430.24,2.0,2018-02-30,NULL)

insert into PPProduits 
values(3000010,10,3,'Appareil photo','sans miroir X-T20 de Fujifilm avec objectif OIS XC 15-45 mm - Argenté','3000010.jpg',1050.24,3,1,NULL,1050.24,2.0,2018-03-30,NULL)

insert into PPProduits 
values(3000020,10,1,'Planche de Skateboard','Ribeiro Morty Pro Foil Rick and Morty 8.0','3000020.jpg',80.00,5,1,NULL,80.00,2.0,2018-03-01,NULL)

insert into PPProduits 
values(3000030,10,80,'Soulier','Vans Souliers Chima Pro 2','3000030.jpg',90.00,5,1,NULL,90.00,5.0,2019-01-01,NULL)

insert into PPProduits 
values(3000040,10,80,'Cotons ouatés','Thrasher Hoodie China Banks','3000040.jpg',65.00,5,1,NULL,65.00,5.0,2019-01-05,NULL)

insert into PPProduits 
values(3000050,20,80,'Boxer','Undz Boxer Print','3000050.jpg',15.00,10,1,NULL,15.00,5.0,2019-01-30,NULL)

insert into PPProduits 
values(4000010,20,80,'Bracelet','Rastaclat Bracelet Miniclat','4000020.jpg',20.00,10,1,NULL,20.00,5.0,2006-01-30,NULL)

update PPProduits set Photo = '3000050.jpg' where NoProduit = 3000050

--Ajouter des commandes
INSERT INTO PPCommandes 
VALUES(1,10000,10,GETDATE(),10.00,1,50.00,2.00,1.50,10.5,'N','CESTQUOICECHAMPS')

INSERT INTO PPDetailsCommandes
VALUES(1,1,1000010,20.00,2)

--Ajouter un historique de paiement
INSERT INTO PPHistoriquePaiements
VALUES(1,45.00,10,10000,1,GETDATE(),'FSJGFD545GDFG',10.00,10.00,5.00,2.00,1.50)

update PPCommandes set Statut = 'N' where NoCommande = 1
