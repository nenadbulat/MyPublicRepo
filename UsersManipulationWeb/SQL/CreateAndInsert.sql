CREATE TABLE user(
    id INT NOT NULL AUTO_INCREMENT,
    username VARCHAR(255),
    fullname VARCHAR(255),
    email VARCHAR(255),
    mobilenumber VARCHAR(255),
    language INT,
    culture INT,
    password VARCHAR(255),
    PRIMARY KEY (id)
);

CREATE TABLE apikeys(
    id VARCHAR(255),
    apikey VARCHAR(255),
    PRIMARY KEY(id)
);

INSERT INTO user VALUES(1, "john.doe", "John Doe", "john.doe@gmail.com", "00381601234567", 6, 7, "John123@456");
INSERT INTO user VALUES(2, "jane.doe", "Jane Doe", "jane.doe@gmail.com", "00381607654321", 1, 2, "Jane234#345");
INSERT INTO user VALUES(3, "nenad.bulat", "Nenad Bulat", "nenad.bulat@gmail.com", "00381603456789", 6, 8, "Nenad456%345");
INSERT INTO user VALUES(4, "teodor.bulat", "Teodor Bulat", "teodor.bulat@gmail.com", "00381609876543", 6, 8, "Teodor789#890");
INSERT INTO user VALUES(5, "snezana.bulat", "Snezana Bulat", "snezana.bulat@gmail.com", "00381601357911", 4, 5, "Snezana456$567");
