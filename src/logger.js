/*
* logger
*/

const os = require('os');
const fs = require('fs');
const path = require('path');

function getTime() {
  const now = new Date();

  const year = now.getFullYear();
  let month = now.getMonth() + 1;
  let day = now.getDate();
  let hour = now.getHours();
  let minute = now.getMinutes();
  let seconds = now.getSeconds();
  let milliseconds = now.getMilliseconds();

  month = (month < 10 ? '0' : '') + month;
  day = (day < 10 ? '0' : '') + day;
  hour = (hour < 10 ? '0' : '') + hour;
  minute = (minute < 10 ? '0' : '') + minute;
  seconds = (seconds < 10 ? '0' : '') + seconds;
  if (milliseconds < 10) {
    milliseconds = `00${milliseconds}`;
  } else if (milliseconds < 100) {
    milliseconds = `0${milliseconds}`;
  }

  return {
    year,
    month,
    day,
    hour,
    minute,
    seconds,
    milliseconds,
  };
}

function getTimestamp(time) {
  return `${time.year}-${time.month}-${time.day} ${time.hour}:${time.minute}:${time.seconds}.${time.milliseconds}`;
}

function getAppdataDir() {
  const appdata = process.env.APPDATA || (process.platform === 'darwin' ? `${process.env.HOME}Library/Preferences` : '/var/local');
  return path.resolve(appdata, 'dscraper', 'log');
}

function ensureExists(dir) {
  const segments = dir.split(path.sep);
  for (let i = 1; i <= segments.length; i += 1) {
    const segment = segments.slice(0, i).join(path.sep);
    if (!fs.existsSync(segment)) {
      fs.mkdirSync(segment);
    }
  }
}

function getDefaultFile(time) {
  const appdata = getAppdataDir();
  const name = `${time.year}-${time.month}-${time.day}.log`;
  return path.resolve(appdata, name);
}

function writeLog(file, type, message) {
  const now = getTime();
  const timestamp = getTimestamp(now);
  const filename = file || getDefaultFile(now);
  const content = `[${timestamp}] ${type} ${message}${os.EOL}`;
  fs.appendFileSync(filename, content);
}

class Logger {
  constructor(file) {
    this.file = file;
  }

  static create(file) {
    return new Logger(file);
  }

  static get log() {
    return Logger.create(null);
  }

  info(message) {
    console.info(message);
    writeLog(this.file, '[info]', message);
  }

  warn(message) {
    console.warn(message);
    writeLog(this.file, '[warn]', message);
  }

  error(message) {
    console.error(message);
    writeLog(this.file, '[error]', message);
  }
}

ensureExists(getAppdataDir());

module.exports = Logger;
