BEGIN;

-- ----------------------------
-- Chat publico
-- ----------------------------

CREATE TABLE IF NOT EXISTS `_bournechannel` (
  `_datetime` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  `_region` varchar(128) NOT NULL DEFAULT '',
  `_channel` int(11) NOT NULL DEFAULT '0',
  `_chattype` varchar(32) NOT NULL DEFAULT '',
  `_x` double DEFAULT NULL,
  `_y` float DEFAULT NULL,
  `_z` float DEFAULT NULL,
  `_speaker` varchar(400) NOT NULL DEFAULT '',
  `_message` varchar(400) NOT NULL DEFAULT '',
  `_id` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`_id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Chat privado
-- ----------------------------

CREATE TABLE IF NOT EXISTS `_bourneim` (
  `_datetime` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  `_region` varchar(128) NOT NULL DEFAULT '',
  `_speaker` varchar(128) NOT NULL DEFAULT '',
  `_listener` varchar(128) NOT NULL DEFAULT '',
  `_message` varchar(400) NOT NULL DEFAULT '',
  `_id` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`_id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Acceso
-- ----------------------------

CREATE TABLE IF NOT EXISTS `_sessions` (
  `start_timestamp` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  `end_timestamp` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  `avatar` varchar(400) NOT NULL DEFAULT '',
  `viewer` varchar(128) NOT NULL DEFAULT '',
  `ip` varchar(128) NOT NULL DEFAULT '',
  `session_id` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`session_id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Tracking del avatar
-- ----------------------------

CREATE TABLE IF NOT EXISTS `_trace` (
  `_avatar` varchar(400) NOT NULL DEFAULT '',
  `_region_id` varchar(128) NOT NULL DEFAULT '',
  `_animation` varchar(128) NOT NULL DEFAULT '',
  `_x` float DEFAULT NULL,
  `_y` float DEFAULT NULL,
  `_z` float DEFAULT NULL,
  `_rotation` float DEFAULT NULL,
  `_datetime` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  PRIMARY KEY (`_avatar`,`_datetime`, `_x`, `_y`, `_z`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;


-- ----------------------------
-- Datos del avatar
-- ----------------------------
CREATE TABLE IF NOT EXISTS `_avatar` (
  `_id` varchar(128) NOT NULL DEFAULT '',
  `first_name` varchar(400) NOT NULL DEFAULT '',
  `last_name` varchar(400) NOT NULL DEFAULT '',
  `creation_date` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  PRIMARY KEY (`_id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;


-- ----------------------------
-- Eventos
-- ----------------------------
CREATE TABLE IF NOT EXISTS `_events` (
  `_id` varchar(128) NOT NULL DEFAULT '',
  `_type` varchar(400) NOT NULL DEFAULT '',
  `_avatar` varchar(400) NOT NULL DEFAULT '',
  `_objeto` varchar(400) NOT NULL DEFAULT '',
  `_region` varchar(400) NOT NULL DEFAULT '',
  `_x` float DEFAULT NULL,
  `_y` float DEFAULT NULL,
  `_z` float DEFAULT NULL,
  `_datetime` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  PRIMARY KEY (`_id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;


-- ----------------------------
-- Acciones
-- ----------------------------
CREATE TABLE IF NOT EXISTS `_actions` (
  `_id` varchar(128) NOT NULL DEFAULT '',
  `_type` varchar(400) NOT NULL DEFAULT '',
  `_avatar` varchar(400) NOT NULL DEFAULT '',
  `_region` varchar(400) NOT NULL DEFAULT '',
  `_x` float DEFAULT NULL,
  `_y` float DEFAULT NULL,
  `_z` float DEFAULT NULL,
  `_datetime` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  PRIMARY KEY (`_id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Regiones
-- ----------------------------

CREATE TABLE IF NOT EXISTS `_regions` (
  `_id` varchar(128) NOT NULL DEFAULT '',
  `_name` varchar(400) NOT NULL DEFAULT '',
  `_x` float DEFAULT NULL,
  `_y` float DEFAULT NULL,
  PRIMARY KEY (`_id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;


-- ----------------------------
-- Teleport
-- ----------------------------

CREATE TABLE IF NOT EXISTS `_teleport` (
  `_avatar` varchar(400) NOT NULL DEFAULT '',
  `_region_id` varchar(128) NOT NULL DEFAULT '',
  `_region_id_dest` varchar(128) NOT NULL DEFAULT '',
  `_x` float DEFAULT NULL,
  `_y` float DEFAULT NULL,
  `_z` float DEFAULT NULL,
  `_datetime` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  PRIMARY KEY (`_avatar`,`_datetime`, `_x`, `_y`, `_z`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;


COMMIT;