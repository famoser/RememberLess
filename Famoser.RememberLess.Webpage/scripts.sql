DROP TABLE Note;
DROP TABLE NoteCollection;
DROP TABLE NoteTakerListRelation;
DROP TABLE NoteTaker;

CREATE TABLE NoteTakers
(
	Id INTEGER PRIMARY KEY AUTOINCREMENT,
	Guid varchar(255)
);

CREATE TABLE NoteTakerNoteCollectionRelations
(
	Id INTEGER PRIMARY KEY AUTOINCREMENT,
	NoteTakerId int,
	NoteCollectionId int
);

CREATE TABLE NoteCollections
(
	Id INTEGER PRIMARY KEY AUTOINCREMENT,
	Guid varchar(255),
	Name text,
	CreateTime datetime
);

CREATE TABLE Notes
(
	Id INTEGER PRIMARY KEY AUTOINCREMENT,
	NoteCollectionId int,
	Guid varchar(255),
	Content text,
	CreateTime datetime,
	IsCompleted bool
);