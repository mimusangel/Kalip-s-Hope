BEGIN TRANSACTION;
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
INSERT INTO `Item` (id,character,template,stats,number,slot) VALUES (4,1,5,'5;10;15|6;10;15|7;10;15|8;10;15|0;42;41|1;-9;-10|2;45;44|3;12;11|4;-20;-21',1,15);
INSERT INTO `Item` (id,character,template,stats,number,slot) VALUES (5,1,5,'5;10;15|6;10;15|7;10;15|8;10;15|0;52;51|1;-35;-36|2;51;50|3;14;13|4;0;-1',1,3);
INSERT INTO `Item` (id,character,template,stats,number,slot) VALUES (9,1,1,'',28,4);
INSERT INTO `Item` (id,character,template,stats,number,slot) VALUES (23,1,5,'5;10;15|6;10;15|7;10;15|8;10;15|0;18;17|1;-27;-28|2;39;38|3;11;10|4;-27;-28',1,8);
COMMIT;
