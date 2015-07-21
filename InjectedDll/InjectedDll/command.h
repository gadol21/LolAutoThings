#ifndef __COMMAND_H
#define __COMMAND_H

class Command {
public:
	/**
	 * this functor will get called on main_loop step.
	 */
	virtual void operator()() = 0;
};

#endif // ndef __COMMAND_H