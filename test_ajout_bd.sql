use BD6B8_424R
use BD6B8_424R_TESTS

select * from PPClients
select * from PPVendeurs

select * from PPCategories
select * from PPProduits
select * from PPArticlesEnPanier

--Ajouter un vendeur
insert into PPVendeurs values(1,'Childish', 'Blanchet-Plante', 'Kevin','Notre-Dame', 'Montréal','QC','J7W8H5', 'Canada','514-453-8546',null,'kev@gmail.com','Password1',10,20,1,10,null,GETDATE(),GETDATE(),1)

--Ajouter des catégories
insert into PPCategories values(1,'Sport et Loisir', 'Équipement sportif')
insert into PPCategories values(2,'Voiture', 'Voiture d''occasion')
insert into PPCategories values(3,'Électronique','Produit Électronique')
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

delete PPVendeurs