#pragma once

class Command {
public:
	/**
	 * this functor will get called on main_loop step.
	 */
	virtual void operator()() = 0;
};
