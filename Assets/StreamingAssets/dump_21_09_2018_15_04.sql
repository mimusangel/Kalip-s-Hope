BEGIN TRANSACTION;
DROP TABLE IF EXISTS `ItemTemplate`;
CREATE TABLE IF NOT EXISTS `ItemTemplate` (
	`id`	INTEGER DEFAULT 0 PRIMARY KEY AUTOINCREMENT UNIQUE,
	`title`	VARCHAR ( 255 ) NOT NULL,
	`description`	VARCHAR ( 255 ),
	`stats`	TEXT,
	`stackable`	INTEGER NOT NULL DEFAULT 0
);
INSERT INTO `ItemTemplate` (id,title,description,stats,stackable) VALUES (0,'Diamant','Un très joli diamant !',NULL,1);
INSERT INTO `ItemTemplate` (id,title,description,stats,stackable) VALUES (1,'Emeraude','Une très jolie émeraude !',NULL,1);
INSERT INTO `ItemTemplate` (id,title,description,stats,stackable) VALUES (2,'Or','Une très jolie pépite d''or !',NULL,1);
INSERT INTO `ItemTemplate` (id,title,description,stats,stackable) VALUES (3,'Pioche en diamant','Une pioche incassable !','0;20;25|1;15;20',0);
INSERT INTO `ItemTemplate` (id,title,description,stats,stackable) VALUES (4,'Pioche en argent','Une pioche puissante !','0;10;15|1;5;10',0);
INSERT INTO `ItemTemplate` (id,title,description,stats,stackable) VALUES (5,'Epée en diamant','Une épée destructrice !','5;10;15|6;10;15|7;10;15|8;10;15|0;10;55|1;-55;55|2;30;55|3;10;30|4;-30;0',0);
DROP TABLE IF EXISTS `Item`;
CREATE TABLE IF NOT EXISTS `Item` (
	`id`	INTEGER DEFAULT 0 PRIMARY KEY AUTOINCREMENT UNIQUE,
	`character`	INTEGER NOT NULL,
	`template`	INTEGER NOT NULL,
	`stats`	TEXT,
	`number`	INTEGER NOT NULL DEFAULT 1,
	`slot`	INTEGER NOT NULL DEFAULT -1
);
INSERT INTO `Item` (id,character,template,stats,number,slot) VALUES (0,1,3,'0;82;0|1;17;0',1,0);
INSERT INTO `Item` (id,character,template,stats,number,slot) VALUES (3,1,5,'5;10;15|6;10;15|7;10;15|8;10;15|0;38;37|1;10;9|2;39;38|3;23;22|4;-10;-11',1,1);
INSERT INTO `Item` (id,character,template,stats,number,slot) VALUES (4,1,5,'5;10;15|6;10;15|7;10;15|8;10;15|0;42;41|1;-9;-10|2;45;44|3;12;11|4;-20;-21',1,2);
INSERT INTO `Item` (id,character,template,stats,number,slot) VALUES (5,1,5,'5;10;15|6;10;15|7;10;15|8;10;15|0;52;51|1;-35;-36|2;51;50|3;14;13|4;0;-1',1,3);
INSERT INTO `Item` (id,character,template,stats,number,slot) VALUES (9,1,1,'',30,4);
INSERT INTO `Item` (id,character,template,stats,number,slot) VALUES (23,1,5,'5;10;15|6;10;15|7;10;15|8;10;15|0;18;17|1;-27;-28|2;39;38|3;11;10|4;-27;-28',1,8);
INSERT INTO `Item` (id,character,template,stats,number,slot) VALUES (24,1,0,'',4,5);
INSERT INTO `Item` (id,character,template,stats,number,slot) VALUES (25,1,3,'0;25;24|1;17;16',1,6);
INSERT INTO `Item` (id,character,template,stats,number,slot) VALUES (26,1,5,'5;10;15|6;10;15|7;10;15|8;10;15|0;26;25|1;28;27|2;38;37|3;28;27|4;-19;-20',1,7);
COMMIT;
