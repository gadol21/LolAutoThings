import win32ui
import win32con
import win32api
import psutil


def get_league_process():
    return [process for process in psutil.process_iter() if process.name == 'League of Legends'][0]


def is_league_running():
    """
    Return if league of legends game is currently running
    """
    try:
        win32ui.FindWindow(None, "League of Legends (TM) Client")
        return True
    except win32ui.error:
        return False


class Engine(object):

    def __init__(self):
        self._process_handle = None

    def attach(self):
        self._process_handle = win32api.OpenProcess(win32con.PROCESS_ALL_ACCESS, False, get_league_process())