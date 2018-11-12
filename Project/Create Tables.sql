/*CREATE TABLE tbl_Store (
	Item_ID int auto_increment primary key not null,
    Item_Name varchar (30) not null,
    Item_Price float not null,
	In_Use bit not null
);*/

/*CREATE TABLE tbl_Highscores (
	Score_ID int auto_increment primary key not null,
    Player_Name varchar (16) not null,
    Highscore float not null
);*/

/*CREATE TABLE tbl_Save_Data (
	Save_ID int auto_increment not null primary key,
    Medals_Earned tinyint not null,
    Current_Medals tinyint not null,
    Current_Gems int unsigned not null,
    Total_Gems_Earned int unsigned not null,
    Unique_Identifier varchar (255) not null
);*/

/*CREATE TABLE tbl_Level_Save_Data (
	Level_Save_ID int auto_increment not null primary key,
    Map_ID tinyint not null,
    Save_ID int not null,
    Medals_Earned tinyint not null,
    foreign key (Map_ID) references tbl_Map_Data(Map_ID),
    foreign key (Save_ID) references tbl_Save_Data(Save_ID)
);*/

/*CREATE TABLE tbl_Map_Data (
	Map_ID tinyint auto_increment primary key not null,
    Level_Name varchar (255) not null
);*/

/*CREATE TABLE tbl_Spawns (
	Spawn_ID int auto_increment not null primary key,
    Enemy_ID int unsigned not null,
    Map_ID tinyint not null
    
);*/

/*CREATE TABLE tbl_Enemy (
	Enemy_ID int auto_increment not null primary key,
    Enemy_Name varchar (255) not null,
    Enemy_Type varchar (50) not null,
    Wave_Count tinyint unsigned not null
);*/