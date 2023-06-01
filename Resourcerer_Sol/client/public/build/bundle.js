
(function(l, r) { if (!l || l.getElementById('livereloadscript')) return; r = l.createElement('script'); r.async = 1; r.src = '//' + (self.location.host || 'localhost').split(':')[0] + ':35729/livereload.js?snipver=1'; r.id = 'livereloadscript'; l.getElementsByTagName('head')[0].appendChild(r) })(self.document);
var app = (function () {
    'use strict';

    var pageService = /*#__PURE__*/Object.freeze({
        __proto__: null,
        get goto () { return goto; },
        get onCurrentPageChanged () { return onCurrentPageChanged; }
    });

    function noop() { }
    function assign(tar, src) {
        // @ts-ignore
        for (const k in src)
            tar[k] = src[k];
        return tar;
    }
    function add_location(element, file, line, column, char) {
        element.__svelte_meta = {
            loc: { file, line, column, char }
        };
    }
    function run(fn) {
        return fn();
    }
    function blank_object() {
        return Object.create(null);
    }
    function run_all(fns) {
        fns.forEach(run);
    }
    function is_function(thing) {
        return typeof thing === 'function';
    }
    function safe_not_equal(a, b) {
        return a != a ? b == b : a !== b || ((a && typeof a === 'object') || typeof a === 'function');
    }
    function is_empty(obj) {
        return Object.keys(obj).length === 0;
    }
    function null_to_empty(value) {
        return value == null ? '' : value;
    }
    function append(target, node) {
        target.appendChild(node);
    }
    function insert(target, node, anchor) {
        target.insertBefore(node, anchor || null);
    }
    function detach(node) {
        if (node.parentNode) {
            node.parentNode.removeChild(node);
        }
    }
    function destroy_each(iterations, detaching) {
        for (let i = 0; i < iterations.length; i += 1) {
            if (iterations[i])
                iterations[i].d(detaching);
        }
    }
    function element(name) {
        return document.createElement(name);
    }
    function text(data) {
        return document.createTextNode(data);
    }
    function space() {
        return text(' ');
    }
    function empty() {
        return text('');
    }
    function listen(node, event, handler, options) {
        node.addEventListener(event, handler, options);
        return () => node.removeEventListener(event, handler, options);
    }
    function prevent_default(fn) {
        return function (event) {
            event.preventDefault();
            // @ts-ignore
            return fn.call(this, event);
        };
    }
    function attr(node, attribute, value) {
        if (value == null)
            node.removeAttribute(attribute);
        else if (node.getAttribute(attribute) !== value)
            node.setAttribute(attribute, value);
    }
    function children(element) {
        return Array.from(element.childNodes);
    }
    function set_input_value(input, value) {
        input.value = value == null ? '' : value;
    }
    function custom_event(type, detail, { bubbles = false, cancelable = false } = {}) {
        const e = document.createEvent('CustomEvent');
        e.initCustomEvent(type, bubbles, cancelable, detail);
        return e;
    }

    let current_component;
    function set_current_component(component) {
        current_component = component;
    }
    function get_current_component() {
        if (!current_component)
            throw new Error('Function called outside component initialization');
        return current_component;
    }
    /**
     * The `onMount` function schedules a callback to run as soon as the component has been mounted to the DOM.
     * It must be called during the component's initialisation (but doesn't need to live *inside* the component;
     * it can be called from an external module).
     *
     * `onMount` does not run inside a [server-side component](/docs#run-time-server-side-component-api).
     *
     * https://svelte.dev/docs#run-time-svelte-onmount
     */
    function onMount(fn) {
        get_current_component().$$.on_mount.push(fn);
    }
    /**
     * Creates an event dispatcher that can be used to dispatch [component events](/docs#template-syntax-component-directives-on-eventname).
     * Event dispatchers are functions that can take two arguments: `name` and `detail`.
     *
     * Component events created with `createEventDispatcher` create a
     * [CustomEvent](https://developer.mozilla.org/en-US/docs/Web/API/CustomEvent).
     * These events do not [bubble](https://developer.mozilla.org/en-US/docs/Learn/JavaScript/Building_blocks/Events#Event_bubbling_and_capture).
     * The `detail` argument corresponds to the [CustomEvent.detail](https://developer.mozilla.org/en-US/docs/Web/API/CustomEvent/detail)
     * property and can contain any type of data.
     *
     * https://svelte.dev/docs#run-time-svelte-createeventdispatcher
     */
    function createEventDispatcher() {
        const component = get_current_component();
        return (type, detail, { cancelable = false } = {}) => {
            const callbacks = component.$$.callbacks[type];
            if (callbacks) {
                // TODO are there situations where events could be dispatched
                // in a server (non-DOM) environment?
                const event = custom_event(type, detail, { cancelable });
                callbacks.slice().forEach(fn => {
                    fn.call(component, event);
                });
                return !event.defaultPrevented;
            }
            return true;
        };
    }

    const dirty_components = [];
    const binding_callbacks = [];
    const render_callbacks = [];
    const flush_callbacks = [];
    const resolved_promise = Promise.resolve();
    let update_scheduled = false;
    function schedule_update() {
        if (!update_scheduled) {
            update_scheduled = true;
            resolved_promise.then(flush);
        }
    }
    function add_render_callback(fn) {
        render_callbacks.push(fn);
    }
    // flush() calls callbacks in this order:
    // 1. All beforeUpdate callbacks, in order: parents before children
    // 2. All bind:this callbacks, in reverse order: children before parents.
    // 3. All afterUpdate callbacks, in order: parents before children. EXCEPT
    //    for afterUpdates called during the initial onMount, which are called in
    //    reverse order: children before parents.
    // Since callbacks might update component values, which could trigger another
    // call to flush(), the following steps guard against this:
    // 1. During beforeUpdate, any updated components will be added to the
    //    dirty_components array and will cause a reentrant call to flush(). Because
    //    the flush index is kept outside the function, the reentrant call will pick
    //    up where the earlier call left off and go through all dirty components. The
    //    current_component value is saved and restored so that the reentrant call will
    //    not interfere with the "parent" flush() call.
    // 2. bind:this callbacks cannot trigger new flush() calls.
    // 3. During afterUpdate, any updated components will NOT have their afterUpdate
    //    callback called a second time; the seen_callbacks set, outside the flush()
    //    function, guarantees this behavior.
    const seen_callbacks = new Set();
    let flushidx = 0; // Do *not* move this inside the flush() function
    function flush() {
        // Do not reenter flush while dirty components are updated, as this can
        // result in an infinite loop. Instead, let the inner flush handle it.
        // Reentrancy is ok afterwards for bindings etc.
        if (flushidx !== 0) {
            return;
        }
        const saved_component = current_component;
        do {
            // first, call beforeUpdate functions
            // and update components
            try {
                while (flushidx < dirty_components.length) {
                    const component = dirty_components[flushidx];
                    flushidx++;
                    set_current_component(component);
                    update$1(component.$$);
                }
            }
            catch (e) {
                // reset dirty state to not end up in a deadlocked state and then rethrow
                dirty_components.length = 0;
                flushidx = 0;
                throw e;
            }
            set_current_component(null);
            dirty_components.length = 0;
            flushidx = 0;
            while (binding_callbacks.length)
                binding_callbacks.pop()();
            // then, once components are updated, call
            // afterUpdate functions. This may cause
            // subsequent updates...
            for (let i = 0; i < render_callbacks.length; i += 1) {
                const callback = render_callbacks[i];
                if (!seen_callbacks.has(callback)) {
                    // ...so guard against infinite loops
                    seen_callbacks.add(callback);
                    callback();
                }
            }
            render_callbacks.length = 0;
        } while (dirty_components.length);
        while (flush_callbacks.length) {
            flush_callbacks.pop()();
        }
        update_scheduled = false;
        seen_callbacks.clear();
        set_current_component(saved_component);
    }
    function update$1($$) {
        if ($$.fragment !== null) {
            $$.update();
            run_all($$.before_update);
            const dirty = $$.dirty;
            $$.dirty = [-1];
            $$.fragment && $$.fragment.p($$.ctx, dirty);
            $$.after_update.forEach(add_render_callback);
        }
    }
    const outroing = new Set();
    let outros;
    function group_outros() {
        outros = {
            r: 0,
            c: [],
            p: outros // parent group
        };
    }
    function check_outros() {
        if (!outros.r) {
            run_all(outros.c);
        }
        outros = outros.p;
    }
    function transition_in(block, local) {
        if (block && block.i) {
            outroing.delete(block);
            block.i(local);
        }
    }
    function transition_out(block, local, detach, callback) {
        if (block && block.o) {
            if (outroing.has(block))
                return;
            outroing.add(block);
            outros.c.push(() => {
                outroing.delete(block);
                if (callback) {
                    if (detach)
                        block.d(1);
                    callback();
                }
            });
            block.o(local);
        }
        else if (callback) {
            callback();
        }
    }

    const globals = (typeof window !== 'undefined'
        ? window
        : typeof globalThis !== 'undefined'
            ? globalThis
            : global);

    function get_spread_update(levels, updates) {
        const update = {};
        const to_null_out = {};
        const accounted_for = { $$scope: 1 };
        let i = levels.length;
        while (i--) {
            const o = levels[i];
            const n = updates[i];
            if (n) {
                for (const key in o) {
                    if (!(key in n))
                        to_null_out[key] = 1;
                }
                for (const key in n) {
                    if (!accounted_for[key]) {
                        update[key] = n[key];
                        accounted_for[key] = 1;
                    }
                }
                levels[i] = n;
            }
            else {
                for (const key in o) {
                    accounted_for[key] = 1;
                }
            }
        }
        for (const key in to_null_out) {
            if (!(key in update))
                update[key] = undefined;
        }
        return update;
    }
    function get_spread_object(spread_props) {
        return typeof spread_props === 'object' && spread_props !== null ? spread_props : {};
    }
    function create_component(block) {
        block && block.c();
    }
    function mount_component(component, target, anchor, customElement) {
        const { fragment, after_update } = component.$$;
        fragment && fragment.m(target, anchor);
        if (!customElement) {
            // onMount happens before the initial afterUpdate
            add_render_callback(() => {
                const new_on_destroy = component.$$.on_mount.map(run).filter(is_function);
                // if the component was destroyed immediately
                // it will update the `$$.on_destroy` reference to `null`.
                // the destructured on_destroy may still reference to the old array
                if (component.$$.on_destroy) {
                    component.$$.on_destroy.push(...new_on_destroy);
                }
                else {
                    // Edge case - component was destroyed immediately,
                    // most likely as a result of a binding initialising
                    run_all(new_on_destroy);
                }
                component.$$.on_mount = [];
            });
        }
        after_update.forEach(add_render_callback);
    }
    function destroy_component(component, detaching) {
        const $$ = component.$$;
        if ($$.fragment !== null) {
            run_all($$.on_destroy);
            $$.fragment && $$.fragment.d(detaching);
            // TODO null out other refs, including component.$$ (but need to
            // preserve final state?)
            $$.on_destroy = $$.fragment = null;
            $$.ctx = [];
        }
    }
    function make_dirty(component, i) {
        if (component.$$.dirty[0] === -1) {
            dirty_components.push(component);
            schedule_update();
            component.$$.dirty.fill(0);
        }
        component.$$.dirty[(i / 31) | 0] |= (1 << (i % 31));
    }
    function init(component, options, instance, create_fragment, not_equal, props, append_styles, dirty = [-1]) {
        const parent_component = current_component;
        set_current_component(component);
        const $$ = component.$$ = {
            fragment: null,
            ctx: [],
            // state
            props,
            update: noop,
            not_equal,
            bound: blank_object(),
            // lifecycle
            on_mount: [],
            on_destroy: [],
            on_disconnect: [],
            before_update: [],
            after_update: [],
            context: new Map(options.context || (parent_component ? parent_component.$$.context : [])),
            // everything else
            callbacks: blank_object(),
            dirty,
            skip_bound: false,
            root: options.target || parent_component.$$.root
        };
        append_styles && append_styles($$.root);
        let ready = false;
        $$.ctx = instance
            ? instance(component, options.props || {}, (i, ret, ...rest) => {
                const value = rest.length ? rest[0] : ret;
                if ($$.ctx && not_equal($$.ctx[i], $$.ctx[i] = value)) {
                    if (!$$.skip_bound && $$.bound[i])
                        $$.bound[i](value);
                    if (ready)
                        make_dirty(component, i);
                }
                return ret;
            })
            : [];
        $$.update();
        ready = true;
        run_all($$.before_update);
        // `false` as a special case of no DOM component
        $$.fragment = create_fragment ? create_fragment($$.ctx) : false;
        if (options.target) {
            if (options.hydrate) {
                const nodes = children(options.target);
                // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
                $$.fragment && $$.fragment.l(nodes);
                nodes.forEach(detach);
            }
            else {
                // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
                $$.fragment && $$.fragment.c();
            }
            if (options.intro)
                transition_in(component.$$.fragment);
            mount_component(component, options.target, options.anchor, options.customElement);
            flush();
        }
        set_current_component(parent_component);
    }
    /**
     * Base class for Svelte components. Used when dev=false.
     */
    class SvelteComponent {
        $destroy() {
            destroy_component(this, 1);
            this.$destroy = noop;
        }
        $on(type, callback) {
            if (!is_function(callback)) {
                return noop;
            }
            const callbacks = (this.$$.callbacks[type] || (this.$$.callbacks[type] = []));
            callbacks.push(callback);
            return () => {
                const index = callbacks.indexOf(callback);
                if (index !== -1)
                    callbacks.splice(index, 1);
            };
        }
        $set($$props) {
            if (this.$$set && !is_empty($$props)) {
                this.$$.skip_bound = true;
                this.$$set($$props);
                this.$$.skip_bound = false;
            }
        }
    }

    function dispatch_dev(type, detail) {
        document.dispatchEvent(custom_event(type, Object.assign({ version: '3.55.1' }, detail), { bubbles: true }));
    }
    function append_dev(target, node) {
        dispatch_dev('SvelteDOMInsert', { target, node });
        append(target, node);
    }
    function insert_dev(target, node, anchor) {
        dispatch_dev('SvelteDOMInsert', { target, node, anchor });
        insert(target, node, anchor);
    }
    function detach_dev(node) {
        dispatch_dev('SvelteDOMRemove', { node });
        detach(node);
    }
    function listen_dev(node, event, handler, options, has_prevent_default, has_stop_propagation) {
        const modifiers = options === true ? ['capture'] : options ? Array.from(Object.keys(options)) : [];
        if (has_prevent_default)
            modifiers.push('preventDefault');
        if (has_stop_propagation)
            modifiers.push('stopPropagation');
        dispatch_dev('SvelteDOMAddEventListener', { node, event, handler, modifiers });
        const dispose = listen(node, event, handler, options);
        return () => {
            dispatch_dev('SvelteDOMRemoveEventListener', { node, event, handler, modifiers });
            dispose();
        };
    }
    function attr_dev(node, attribute, value) {
        attr(node, attribute, value);
        if (value == null)
            dispatch_dev('SvelteDOMRemoveAttribute', { node, attribute });
        else
            dispatch_dev('SvelteDOMSetAttribute', { node, attribute, value });
    }
    function set_data_dev(text, data) {
        data = '' + data;
        if (text.wholeText === data)
            return;
        dispatch_dev('SvelteDOMSetData', { node: text, data });
        text.data = data;
    }
    function validate_each_argument(arg) {
        if (typeof arg !== 'string' && !(arg && typeof arg === 'object' && 'length' in arg)) {
            let msg = '{#each} only iterates over array-like objects.';
            if (typeof Symbol === 'function' && arg && Symbol.iterator in arg) {
                msg += ' You can use a spread to convert this iterable into an array.';
            }
            throw new Error(msg);
        }
    }
    function validate_slots(name, slot, keys) {
        for (const slot_key of Object.keys(slot)) {
            if (!~keys.indexOf(slot_key)) {
                console.warn(`<${name}> received an unexpected slot "${slot_key}".`);
            }
        }
    }
    function construct_svelte_component_dev(component, props) {
        const error_message = 'this={...} of <svelte:component> should specify a Svelte component.';
        try {
            const instance = new component(props);
            if (!instance.$$ || !instance.$set || !instance.$on || !instance.$destroy) {
                throw new Error(error_message);
            }
            return instance;
        }
        catch (err) {
            const { message } = err;
            if (typeof message === 'string' && message.indexOf('is not a constructor') !== -1) {
                throw new Error(error_message);
            }
            else {
                throw err;
            }
        }
    }
    /**
     * Base class for Svelte components with some minor dev-enhancements. Used when dev=true.
     */
    class SvelteComponentDev extends SvelteComponent {
        constructor(options) {
            if (!options || (!options.target && !options.$$inline)) {
                throw new Error("'target' is a required option");
            }
            super();
        }
        $destroy() {
            super.$destroy();
            this.$destroy = () => {
                console.warn('Component was already destroyed'); // eslint-disable-line no-console
            };
        }
        $capture_state() { }
        $inject_state() { }
    }

    const subscriber_queue = [];
    /**
     * Create a `Writable` store that allows both updating and reading by subscription.
     * @param {*=}value initial value
     * @param {StartStopNotifier=}start start and stop notifications for subscriptions
     */
    function writable(value, start = noop) {
        let stop;
        const subscribers = new Set();
        function set(new_value) {
            if (safe_not_equal(value, new_value)) {
                value = new_value;
                if (stop) { // store is ready
                    const run_queue = !subscriber_queue.length;
                    for (const subscriber of subscribers) {
                        subscriber[1]();
                        subscriber_queue.push(subscriber, value);
                    }
                    if (run_queue) {
                        for (let i = 0; i < subscriber_queue.length; i += 2) {
                            subscriber_queue[i][0](subscriber_queue[i + 1]);
                        }
                        subscriber_queue.length = 0;
                    }
                }
            }
        }
        function update(fn) {
            set(fn(value));
        }
        function subscribe(run, invalidate = noop) {
            const subscriber = [run, invalidate];
            subscribers.add(subscriber);
            if (subscribers.size === 1) {
                stop = start(set) || noop;
            }
            run(value);
            return () => {
                subscribers.delete(subscriber);
                if (subscribers.size === 0) {
                    stop();
                    stop = null;
                }
            };
        }
        return { set, update, subscribe };
    }

    let callCount = 0;
    const options$ = writable({ open: false, message: 'loading', progress: null });
    const options = options$.subscribe;
    function show(message, progress) {
        callCount += 1;
        options$.set({
            open: true,
            message: message !== null && message !== void 0 ? message : 'loading',
            progress: progress
        });
    }
    function update(message, progress) {
        options$.set({
            open: true,
            message: message,
            progress: progress
        });
    }
    function hide() {
        callCount -= 1;
        if (callCount === 0) {
            options$.set({
                open: false,
                message: 'loading',
                progress: 0
            });
        }
    }

    var pageLoaderService = /*#__PURE__*/Object.freeze({
        __proto__: null,
        hide: hide,
        options: options,
        show: show,
        update: update
    });

    /* src\AppTest.svelte generated by Svelte v3.55.1 */

    const { console: console_1$3 } = globals;
    const file$b = "src\\AppTest.svelte";

    function create_fragment$c(ctx) {
    	let button;
    	let mounted;
    	let dispose;

    	const block = {
    		c: function create() {
    			button = element("button");
    			button.textContent = "Test loader";
    			add_location(button, file$b, 13, 0, 389);
    		},
    		l: function claim(nodes) {
    			throw new Error("options.hydrate only works if the component was compiled with the `hydratable: true` option");
    		},
    		m: function mount(target, anchor) {
    			insert_dev(target, button, anchor);

    			if (!mounted) {
    				dispose = listen_dev(button, "click", /*testLoader*/ ctx[0], false, false, false);
    				mounted = true;
    			}
    		},
    		p: noop,
    		i: noop,
    		o: noop,
    		d: function destroy(detaching) {
    			if (detaching) detach_dev(button);
    			mounted = false;
    			dispose();
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_fragment$c.name,
    		type: "component",
    		source: "",
    		ctx
    	});

    	return block;
    }

    function instance$c($$self, $$props, $$invalidate) {
    	let { $$slots: slots = {}, $$scope } = $$props;
    	validate_slots('AppTest', slots, []);

    	function testLoader() {
    		[2500, 1000, 1500].forEach(x => {
    			show();

    			const tout = setTimeout(
    				() => {
    					hide();
    					console.log(`task ${x} done`);
    					clearTimeout(tout);
    				},
    				x
    			);
    		});
    	}

    	const writable_props = [];

    	Object.keys($$props).forEach(key => {
    		if (!~writable_props.indexOf(key) && key.slice(0, 2) !== '$$' && key !== 'slot') console_1$3.warn(`<AppTest> was created with unknown prop '${key}'`);
    	});

    	$$self.$capture_state = () => ({ pageLoaderService, testLoader });
    	return [testLoader];
    }

    class AppTest extends SvelteComponentDev {
    	constructor(options) {
    		super(options);
    		init(this, options, instance$c, create_fragment$c, safe_not_equal, {});

    		dispatch_dev("SvelteRegisterComponent", {
    			component: this,
    			tagName: "AppTest",
    			options,
    			id: create_fragment$c.name
    		});
    	}
    }

    /* src\components\commonUI\AppHeader.svelte generated by Svelte v3.55.1 */

    const file$a = "src\\components\\commonUI\\AppHeader.svelte";

    function create_fragment$b(ctx) {
    	let header;
    	let h1;

    	const block = {
    		c: function create() {
    			header = element("header");
    			h1 = element("h1");
    			h1.textContent = "Resourcerer";
    			attr_dev(h1, "class", "svelte-iit3b7");
    			add_location(h1, file$a, 3, 4, 45);
    			attr_dev(header, "class", "svelte-iit3b7");
    			add_location(header, file$a, 2, 0, 31);
    		},
    		l: function claim(nodes) {
    			throw new Error("options.hydrate only works if the component was compiled with the `hydratable: true` option");
    		},
    		m: function mount(target, anchor) {
    			insert_dev(target, header, anchor);
    			append_dev(header, h1);
    		},
    		p: noop,
    		i: noop,
    		o: noop,
    		d: function destroy(detaching) {
    			if (detaching) detach_dev(header);
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_fragment$b.name,
    		type: "component",
    		source: "",
    		ctx
    	});

    	return block;
    }

    function instance$b($$self, $$props) {
    	let { $$slots: slots = {}, $$scope } = $$props;
    	validate_slots('AppHeader', slots, []);
    	const writable_props = [];

    	Object.keys($$props).forEach(key => {
    		if (!~writable_props.indexOf(key) && key.slice(0, 2) !== '$$' && key !== 'slot') console.warn(`<AppHeader> was created with unknown prop '${key}'`);
    	});

    	return [];
    }

    class AppHeader extends SvelteComponentDev {
    	constructor(options) {
    		super(options);
    		init(this, options, instance$b, create_fragment$b, safe_not_equal, {});

    		dispatch_dev("SvelteRegisterComponent", {
    			component: this,
    			tagName: "AppHeader",
    			options,
    			id: create_fragment$b.name
    		});
    	}
    }

    /* src\components\commonUI\PageLoader.svelte generated by Svelte v3.55.1 */
    const file$9 = "src\\components\\commonUI\\PageLoader.svelte";

    // (9:0) {#if options?.open}
    function create_if_block$5(ctx) {
    	let div4;
    	let div3;
    	let div2;
    	let div0;
    	let t0;
    	let div1;
    	let t1;
    	let t2;
    	let if_block0 = /*options*/ ctx[0]?.message && create_if_block_2(ctx);
    	let if_block1 = /*options*/ ctx[0]?.progress && create_if_block_1(ctx);

    	const block = {
    		c: function create() {
    			div4 = element("div");
    			div3 = element("div");
    			div2 = element("div");
    			div0 = element("div");
    			t0 = space();
    			div1 = element("div");
    			t1 = space();
    			if (if_block0) if_block0.c();
    			t2 = space();
    			if (if_block1) if_block1.c();
    			attr_dev(div0, "class", "svelte-q3c992");
    			add_location(div0, file$9, 12, 16, 373);
    			attr_dev(div1, "class", "svelte-q3c992");
    			add_location(div1, file$9, 13, 16, 402);
    			attr_dev(div2, "class", "lds-ripple svelte-q3c992");
    			add_location(div2, file$9, 11, 12, 331);
    			attr_dev(div3, "class", "container svelte-q3c992");
    			add_location(div3, file$9, 10, 8, 294);
    			attr_dev(div4, "class", "background svelte-q3c992");
    			add_location(div4, file$9, 9, 4, 260);
    		},
    		m: function mount(target, anchor) {
    			insert_dev(target, div4, anchor);
    			append_dev(div4, div3);
    			append_dev(div3, div2);
    			append_dev(div2, div0);
    			append_dev(div2, t0);
    			append_dev(div2, div1);
    			append_dev(div4, t1);
    			if (if_block0) if_block0.m(div4, null);
    			append_dev(div4, t2);
    			if (if_block1) if_block1.m(div4, null);
    		},
    		p: function update(ctx, dirty) {
    			if (/*options*/ ctx[0]?.message) {
    				if (if_block0) {
    					if_block0.p(ctx, dirty);
    				} else {
    					if_block0 = create_if_block_2(ctx);
    					if_block0.c();
    					if_block0.m(div4, t2);
    				}
    			} else if (if_block0) {
    				if_block0.d(1);
    				if_block0 = null;
    			}

    			if (/*options*/ ctx[0]?.progress) {
    				if (if_block1) {
    					if_block1.p(ctx, dirty);
    				} else {
    					if_block1 = create_if_block_1(ctx);
    					if_block1.c();
    					if_block1.m(div4, null);
    				}
    			} else if (if_block1) {
    				if_block1.d(1);
    				if_block1 = null;
    			}
    		},
    		d: function destroy(detaching) {
    			if (detaching) detach_dev(div4);
    			if (if_block0) if_block0.d();
    			if (if_block1) if_block1.d();
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_if_block$5.name,
    		type: "if",
    		source: "(9:0) {#if options?.open}",
    		ctx
    	});

    	return block;
    }

    // (18:8) {#if options?.message}
    function create_if_block_2(ctx) {
    	let div;
    	let t_value = /*options*/ ctx[0].message + "";
    	let t;

    	const block = {
    		c: function create() {
    			div = element("div");
    			t = text(t_value);
    			attr_dev(div, "class", "info-box svelte-q3c992");
    			add_location(div, file$9, 18, 12, 497);
    		},
    		m: function mount(target, anchor) {
    			insert_dev(target, div, anchor);
    			append_dev(div, t);
    		},
    		p: function update(ctx, dirty) {
    			if (dirty & /*options*/ 1 && t_value !== (t_value = /*options*/ ctx[0].message + "")) set_data_dev(t, t_value);
    		},
    		d: function destroy(detaching) {
    			if (detaching) detach_dev(div);
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_if_block_2.name,
    		type: "if",
    		source: "(18:8) {#if options?.message}",
    		ctx
    	});

    	return block;
    }

    // (21:8) {#if options?.progress}
    function create_if_block_1(ctx) {
    	let div;
    	let t_value = /*options*/ ctx[0].progress + "";
    	let t;

    	const block = {
    		c: function create() {
    			div = element("div");
    			t = text(t_value);
    			attr_dev(div, "class", "info-box svelte-q3c992");
    			add_location(div, file$9, 21, 12, 604);
    		},
    		m: function mount(target, anchor) {
    			insert_dev(target, div, anchor);
    			append_dev(div, t);
    		},
    		p: function update(ctx, dirty) {
    			if (dirty & /*options*/ 1 && t_value !== (t_value = /*options*/ ctx[0].progress + "")) set_data_dev(t, t_value);
    		},
    		d: function destroy(detaching) {
    			if (detaching) detach_dev(div);
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_if_block_1.name,
    		type: "if",
    		source: "(21:8) {#if options?.progress}",
    		ctx
    	});

    	return block;
    }

    function create_fragment$a(ctx) {
    	let if_block_anchor;
    	let if_block = /*options*/ ctx[0]?.open && create_if_block$5(ctx);

    	const block = {
    		c: function create() {
    			if (if_block) if_block.c();
    			if_block_anchor = empty();
    		},
    		l: function claim(nodes) {
    			throw new Error("options.hydrate only works if the component was compiled with the `hydratable: true` option");
    		},
    		m: function mount(target, anchor) {
    			if (if_block) if_block.m(target, anchor);
    			insert_dev(target, if_block_anchor, anchor);
    		},
    		p: function update(ctx, [dirty]) {
    			if (/*options*/ ctx[0]?.open) {
    				if (if_block) {
    					if_block.p(ctx, dirty);
    				} else {
    					if_block = create_if_block$5(ctx);
    					if_block.c();
    					if_block.m(if_block_anchor.parentNode, if_block_anchor);
    				}
    			} else if (if_block) {
    				if_block.d(1);
    				if_block = null;
    			}
    		},
    		i: noop,
    		o: noop,
    		d: function destroy(detaching) {
    			if (if_block) if_block.d(detaching);
    			if (detaching) detach_dev(if_block_anchor);
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_fragment$a.name,
    		type: "component",
    		source: "",
    		ctx
    	});

    	return block;
    }

    function instance$a($$self, $$props, $$invalidate) {
    	let { $$slots: slots = {}, $$scope } = $$props;
    	validate_slots('PageLoader', slots, []);
    	let options$1 = {};

    	onMount(() => {
    		options(x => $$invalidate(0, options$1 = x));
    	});

    	const writable_props = [];

    	Object.keys($$props).forEach(key => {
    		if (!~writable_props.indexOf(key) && key.slice(0, 2) !== '$$' && key !== 'slot') console.warn(`<PageLoader> was created with unknown prop '${key}'`);
    	});

    	$$self.$capture_state = () => ({ onMount, pageLoaderService, options: options$1 });

    	$$self.$inject_state = $$props => {
    		if ('options' in $$props) $$invalidate(0, options$1 = $$props.options);
    	};

    	if ($$props && "$$inject" in $$props) {
    		$$self.$inject_state($$props.$$inject);
    	}

    	return [options$1];
    }

    class PageLoader extends SvelteComponentDev {
    	constructor(options) {
    		super(options);
    		init(this, options, instance$a, create_fragment$a, safe_not_equal, {});

    		dispatch_dev("SvelteRegisterComponent", {
    			component: this,
    			tagName: "PageLoader",
    			options,
    			id: create_fragment$a.name
    		});
    	}
    }

    /* src\components\home\HomeNav.svelte generated by Svelte v3.55.1 */
    const file$8 = "src\\components\\home\\HomeNav.svelte";

    function get_each_context$3(ctx, list, i) {
    	const child_ctx = ctx.slice();
    	child_ctx[7] = list[i];
    	return child_ctx;
    }

    // (25:4) {#each buttons as btn}
    function create_each_block$3(ctx) {
    	let button;
    	let span;
    	let t0_value = /*btn*/ ctx[7].text + "";
    	let t0;
    	let t1;
    	let i;
    	let button_class_value;
    	let mounted;
    	let dispose;

    	function click_handler() {
    		return /*click_handler*/ ctx[5](/*btn*/ ctx[7]);
    	}

    	const block = {
    		c: function create() {
    			button = element("button");
    			span = element("span");
    			t0 = text(t0_value);
    			t1 = space();
    			i = element("i");
    			attr_dev(span, "class", "svelte-a7cxji");
    			add_location(span, file$8, 26, 12, 919);
    			attr_dev(i, "class", "" + (null_to_empty(/*btn*/ ctx[7].icon) + " svelte-a7cxji"));
    			add_location(i, file$8, 27, 12, 956);

    			attr_dev(button, "class", button_class_value = "" + (null_to_empty(/*selectedBtnText*/ ctx[1] === /*btn*/ ctx[7].text
    			? 'marked'
    			: '') + " svelte-a7cxji"));

    			add_location(button, file$8, 25, 8, 799);
    		},
    		m: function mount(target, anchor) {
    			insert_dev(target, button, anchor);
    			append_dev(button, span);
    			append_dev(span, t0);
    			append_dev(button, t1);
    			append_dev(button, i);

    			if (!mounted) {
    				dispose = listen_dev(button, "click", click_handler, false, false, false);
    				mounted = true;
    			}
    		},
    		p: function update(new_ctx, dirty) {
    			ctx = new_ctx;

    			if (dirty & /*selectedBtnText*/ 2 && button_class_value !== (button_class_value = "" + (null_to_empty(/*selectedBtnText*/ ctx[1] === /*btn*/ ctx[7].text
    			? 'marked'
    			: '') + " svelte-a7cxji"))) {
    				attr_dev(button, "class", button_class_value);
    			}
    		},
    		d: function destroy(detaching) {
    			if (detaching) detach_dev(button);
    			mounted = false;
    			dispose();
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_each_block$3.name,
    		type: "each",
    		source: "(25:4) {#each buttons as btn}",
    		ctx
    	});

    	return block;
    }

    // (35:8) {:else}
    function create_else_block$1(ctx) {
    	let i;

    	const block = {
    		c: function create() {
    			i = element("i");
    			attr_dev(i, "class", "las la-angle-right svelte-a7cxji");
    			add_location(i, file$8, 35, 12, 1171);
    		},
    		m: function mount(target, anchor) {
    			insert_dev(target, i, anchor);
    		},
    		d: function destroy(detaching) {
    			if (detaching) detach_dev(i);
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_else_block$1.name,
    		type: "else",
    		source: "(35:8) {:else}",
    		ctx
    	});

    	return block;
    }

    // (33:8) {#if expanded}
    function create_if_block$4(ctx) {
    	let i;

    	const block = {
    		c: function create() {
    			i = element("i");
    			attr_dev(i, "class", "las la-angle-left svelte-a7cxji");
    			add_location(i, file$8, 33, 12, 1107);
    		},
    		m: function mount(target, anchor) {
    			insert_dev(target, i, anchor);
    		},
    		d: function destroy(detaching) {
    			if (detaching) detach_dev(i);
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_if_block$4.name,
    		type: "if",
    		source: "(33:8) {#if expanded}",
    		ctx
    	});

    	return block;
    }

    function create_fragment$9(ctx) {
    	let nav;
    	let t;
    	let button;
    	let nav_class_value;
    	let mounted;
    	let dispose;
    	let each_value = /*buttons*/ ctx[2];
    	validate_each_argument(each_value);
    	let each_blocks = [];

    	for (let i = 0; i < each_value.length; i += 1) {
    		each_blocks[i] = create_each_block$3(get_each_context$3(ctx, each_value, i));
    	}

    	function select_block_type(ctx, dirty) {
    		if (/*expanded*/ ctx[0]) return create_if_block$4;
    		return create_else_block$1;
    	}

    	let current_block_type = select_block_type(ctx);
    	let if_block = current_block_type(ctx);

    	const block = {
    		c: function create() {
    			nav = element("nav");

    			for (let i = 0; i < each_blocks.length; i += 1) {
    				each_blocks[i].c();
    			}

    			t = space();
    			button = element("button");
    			if_block.c();
    			attr_dev(button, "class", "expander svelte-a7cxji");
    			add_location(button, file$8, 31, 4, 1022);
    			attr_dev(nav, "class", nav_class_value = "" + (null_to_empty(/*expanded*/ ctx[0] ? 'expanded' : 'collapsed') + " svelte-a7cxji"));
    			add_location(nav, file$8, 23, 0, 710);
    		},
    		l: function claim(nodes) {
    			throw new Error("options.hydrate only works if the component was compiled with the `hydratable: true` option");
    		},
    		m: function mount(target, anchor) {
    			insert_dev(target, nav, anchor);

    			for (let i = 0; i < each_blocks.length; i += 1) {
    				each_blocks[i].m(nav, null);
    			}

    			append_dev(nav, t);
    			append_dev(nav, button);
    			if_block.m(button, null);

    			if (!mounted) {
    				dispose = listen_dev(button, "click", /*toggleView*/ ctx[4], false, false, false);
    				mounted = true;
    			}
    		},
    		p: function update(ctx, [dirty]) {
    			if (dirty & /*selectedBtnText, buttons, changeComponent*/ 14) {
    				each_value = /*buttons*/ ctx[2];
    				validate_each_argument(each_value);
    				let i;

    				for (i = 0; i < each_value.length; i += 1) {
    					const child_ctx = get_each_context$3(ctx, each_value, i);

    					if (each_blocks[i]) {
    						each_blocks[i].p(child_ctx, dirty);
    					} else {
    						each_blocks[i] = create_each_block$3(child_ctx);
    						each_blocks[i].c();
    						each_blocks[i].m(nav, t);
    					}
    				}

    				for (; i < each_blocks.length; i += 1) {
    					each_blocks[i].d(1);
    				}

    				each_blocks.length = each_value.length;
    			}

    			if (current_block_type !== (current_block_type = select_block_type(ctx))) {
    				if_block.d(1);
    				if_block = current_block_type(ctx);

    				if (if_block) {
    					if_block.c();
    					if_block.m(button, null);
    				}
    			}

    			if (dirty & /*expanded*/ 1 && nav_class_value !== (nav_class_value = "" + (null_to_empty(/*expanded*/ ctx[0] ? 'expanded' : 'collapsed') + " svelte-a7cxji"))) {
    				attr_dev(nav, "class", nav_class_value);
    			}
    		},
    		i: noop,
    		o: noop,
    		d: function destroy(detaching) {
    			if (detaching) detach_dev(nav);
    			destroy_each(each_blocks, detaching);
    			if_block.d();
    			mounted = false;
    			dispose();
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_fragment$9.name,
    		type: "component",
    		source: "",
    		ctx
    	});

    	return block;
    }

    function instance$9($$self, $$props, $$invalidate) {
    	let { $$slots: slots = {}, $$scope } = $$props;
    	validate_slots('HomeNav', slots, []);
    	const dispatch = createEventDispatcher();
    	let expanded = false;

    	const buttons = [
    		{
    			text: 'Categories',
    			icon: 'las la-clipboard-list'
    		},
    		{ text: 'Elements', icon: 'las la-vial' },
    		{ text: 'Composites', icon: 'las la-cubes' },
    		{ text: 'Stocks', icon: 'las la-warehouse' },
    		{ text: 'Settings', icon: 'las la-wrench' },
    		{ text: 'Users', icon: 'las la-users' }
    	];

    	let selectedBtnText = buttons[0].text;

    	function changeComponent(btnText) {
    		$$invalidate(1, selectedBtnText = btnText);
    		dispatch('componentSelected', { name: btnText });
    	}

    	function toggleView() {
    		$$invalidate(0, expanded = !expanded);
    	}

    	const writable_props = [];

    	Object.keys($$props).forEach(key => {
    		if (!~writable_props.indexOf(key) && key.slice(0, 2) !== '$$' && key !== 'slot') console.warn(`<HomeNav> was created with unknown prop '${key}'`);
    	});

    	const click_handler = btn => changeComponent(btn.text);

    	$$self.$capture_state = () => ({
    		createEventDispatcher,
    		dispatch,
    		expanded,
    		buttons,
    		selectedBtnText,
    		changeComponent,
    		toggleView
    	});

    	$$self.$inject_state = $$props => {
    		if ('expanded' in $$props) $$invalidate(0, expanded = $$props.expanded);
    		if ('selectedBtnText' in $$props) $$invalidate(1, selectedBtnText = $$props.selectedBtnText);
    	};

    	if ($$props && "$$inject" in $$props) {
    		$$self.$inject_state($$props.$$inject);
    	}

    	return [expanded, selectedBtnText, buttons, changeComponent, toggleView, click_handler];
    }

    class HomeNav extends SvelteComponentDev {
    	constructor(options) {
    		super(options);
    		init(this, options, instance$9, create_fragment$9, safe_not_equal, {});

    		dispatch_dev("SvelteRegisterComponent", {
    			component: this,
    			tagName: "HomeNav",
    			options,
    			id: create_fragment$9.name
    		});
    	}
    }

    /* src\components\category\CategoryDetails.svelte generated by Svelte v3.55.1 */

    const file$7 = "src\\components\\category\\CategoryDetails.svelte";

    // (8:0) {#if category}
    function create_if_block$3(ctx) {
    	let div2;
    	let div0;
    	let button0;
    	let t0;
    	let button0_class_value;
    	let t1;
    	let button1;
    	let t2;
    	let button1_class_value;
    	let t3;
    	let div1;
    	let mounted;
    	let dispose;

    	const block = {
    		c: function create() {
    			div2 = element("div");
    			div0 = element("div");
    			button0 = element("button");
    			t0 = text("Elements");
    			t1 = space();
    			button1 = element("button");
    			t2 = text("Composites");
    			t3 = space();
    			div1 = element("div");
    			attr_dev(button0, "class", button0_class_value = "" + (null_to_empty(/*focusedBtn*/ ctx[1] === 0 ? 'focused' : '') + " svelte-ih41ll"));
    			add_location(button0, file$7, 10, 12, 210);
    			attr_dev(button1, "class", button1_class_value = "" + (null_to_empty(/*focusedBtn*/ ctx[1] === 1 ? 'focused' : '') + " svelte-ih41ll"));
    			add_location(button1, file$7, 13, 12, 354);
    			attr_dev(div0, "class", "btn-flex svelte-ih41ll");
    			add_location(div0, file$7, 9, 8, 174);
    			add_location(div1, file$7, 17, 8, 512);
    			add_location(div2, file$7, 8, 4, 159);
    		},
    		m: function mount(target, anchor) {
    			insert_dev(target, div2, anchor);
    			append_dev(div2, div0);
    			append_dev(div0, button0);
    			append_dev(button0, t0);
    			append_dev(div0, t1);
    			append_dev(div0, button1);
    			append_dev(button1, t2);
    			append_dev(div2, t3);
    			append_dev(div2, div1);

    			if (!mounted) {
    				dispose = [
    					listen_dev(button0, "click", /*click_handler*/ ctx[3], false, false, false),
    					listen_dev(button1, "click", /*click_handler_1*/ ctx[4], false, false, false)
    				];

    				mounted = true;
    			}
    		},
    		p: function update(ctx, dirty) {
    			if (dirty & /*focusedBtn*/ 2 && button0_class_value !== (button0_class_value = "" + (null_to_empty(/*focusedBtn*/ ctx[1] === 0 ? 'focused' : '') + " svelte-ih41ll"))) {
    				attr_dev(button0, "class", button0_class_value);
    			}

    			if (dirty & /*focusedBtn*/ 2 && button1_class_value !== (button1_class_value = "" + (null_to_empty(/*focusedBtn*/ ctx[1] === 1 ? 'focused' : '') + " svelte-ih41ll"))) {
    				attr_dev(button1, "class", button1_class_value);
    			}
    		},
    		d: function destroy(detaching) {
    			if (detaching) detach_dev(div2);
    			mounted = false;
    			run_all(dispose);
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_if_block$3.name,
    		type: "if",
    		source: "(8:0) {#if category}",
    		ctx
    	});

    	return block;
    }

    function create_fragment$8(ctx) {
    	let if_block_anchor;
    	let if_block = /*category*/ ctx[0] && create_if_block$3(ctx);

    	const block = {
    		c: function create() {
    			if (if_block) if_block.c();
    			if_block_anchor = empty();
    		},
    		l: function claim(nodes) {
    			throw new Error("options.hydrate only works if the component was compiled with the `hydratable: true` option");
    		},
    		m: function mount(target, anchor) {
    			if (if_block) if_block.m(target, anchor);
    			insert_dev(target, if_block_anchor, anchor);
    		},
    		p: function update(ctx, [dirty]) {
    			if (/*category*/ ctx[0]) {
    				if (if_block) {
    					if_block.p(ctx, dirty);
    				} else {
    					if_block = create_if_block$3(ctx);
    					if_block.c();
    					if_block.m(if_block_anchor.parentNode, if_block_anchor);
    				}
    			} else if (if_block) {
    				if_block.d(1);
    				if_block = null;
    			}
    		},
    		i: noop,
    		o: noop,
    		d: function destroy(detaching) {
    			if (if_block) if_block.d(detaching);
    			if (detaching) detach_dev(if_block_anchor);
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_fragment$8.name,
    		type: "component",
    		source: "",
    		ctx
    	});

    	return block;
    }

    function instance$8($$self, $$props, $$invalidate) {
    	let { $$slots: slots = {}, $$scope } = $$props;
    	validate_slots('CategoryDetails', slots, []);
    	let { category = null } = $$props;
    	let focusedBtn = 0;

    	function setFocus(btnIdx) {
    		$$invalidate(1, focusedBtn = btnIdx);
    	}

    	const writable_props = ['category'];

    	Object.keys($$props).forEach(key => {
    		if (!~writable_props.indexOf(key) && key.slice(0, 2) !== '$$' && key !== 'slot') console.warn(`<CategoryDetails> was created with unknown prop '${key}'`);
    	});

    	const click_handler = () => setFocus(0);
    	const click_handler_1 = () => setFocus(1);

    	$$self.$$set = $$props => {
    		if ('category' in $$props) $$invalidate(0, category = $$props.category);
    	};

    	$$self.$capture_state = () => ({ category, focusedBtn, setFocus });

    	$$self.$inject_state = $$props => {
    		if ('category' in $$props) $$invalidate(0, category = $$props.category);
    		if ('focusedBtn' in $$props) $$invalidate(1, focusedBtn = $$props.focusedBtn);
    	};

    	if ($$props && "$$inject" in $$props) {
    		$$self.$inject_state($$props.$$inject);
    	}

    	return [category, focusedBtn, setFocus, click_handler, click_handler_1];
    }

    class CategoryDetails extends SvelteComponentDev {
    	constructor(options) {
    		super(options);
    		init(this, options, instance$8, create_fragment$8, safe_not_equal, { category: 0 });

    		dispatch_dev("SvelteRegisterComponent", {
    			component: this,
    			tagName: "CategoryDetails",
    			options,
    			id: create_fragment$8.name
    		});
    	}

    	get category() {
    		throw new Error("<CategoryDetails>: Props cannot be read directly from the component instance unless compiling with 'accessors: true' or '<svelte:options accessors/>'");
    	}

    	set category(value) {
    		throw new Error("<CategoryDetails>: Props cannot be set directly on the component instance unless compiling with 'accessors: true' or '<svelte:options accessors/>'");
    	}
    }

    const selectedCategoryId$ = writable('');

    var categoryService = /*#__PURE__*/Object.freeze({
        __proto__: null,
        selectedCategoryId$: selectedCategoryId$
    });

    /* src\components\category\CategoryDropdown.svelte generated by Svelte v3.55.1 */
    const file$6 = "src\\components\\category\\CategoryDropdown.svelte";

    function get_each_context$2(ctx, list, i) {
    	const child_ctx = ctx.slice();
    	child_ctx[6] = list[i];
    	return child_ctx;
    }

    // (23:8) {:else}
    function create_else_block(ctx) {
    	let i;

    	const block = {
    		c: function create() {
    			i = element("i");
    			attr_dev(i, "class", "las la-angle-down");
    			add_location(i, file$6, 23, 12, 796);
    		},
    		m: function mount(target, anchor) {
    			insert_dev(target, i, anchor);
    		},
    		d: function destroy(detaching) {
    			if (detaching) detach_dev(i);
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_else_block.name,
    		type: "else",
    		source: "(23:8) {:else}",
    		ctx
    	});

    	return block;
    }

    // (21:8) {#if !expanded}
    function create_if_block$2(ctx) {
    	let i;

    	const block = {
    		c: function create() {
    			i = element("i");
    			attr_dev(i, "class", "las la-angle-right");
    			add_location(i, file$6, 21, 12, 731);
    		},
    		m: function mount(target, anchor) {
    			insert_dev(target, i, anchor);
    		},
    		d: function destroy(detaching) {
    			if (detaching) detach_dev(i);
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_if_block$2.name,
    		type: "if",
    		source: "(21:8) {#if !expanded}",
    		ctx
    	});

    	return block;
    }

    // (30:8) {#each childCategories as child}
    function create_each_block$2(ctx) {
    	let categorydropdown;
    	let current;

    	categorydropdown = new CategoryDropdown({
    			props: {
    				category: /*child*/ ctx[6],
    				allCategories: /*allCategories*/ ctx[1]
    			},
    			$$inline: true
    		});

    	const block = {
    		c: function create() {
    			create_component(categorydropdown.$$.fragment);
    		},
    		m: function mount(target, anchor) {
    			mount_component(categorydropdown, target, anchor);
    			current = true;
    		},
    		p: function update(ctx, dirty) {
    			const categorydropdown_changes = {};
    			if (dirty & /*allCategories*/ 2) categorydropdown_changes.allCategories = /*allCategories*/ ctx[1];
    			categorydropdown.$set(categorydropdown_changes);
    		},
    		i: function intro(local) {
    			if (current) return;
    			transition_in(categorydropdown.$$.fragment, local);
    			current = true;
    		},
    		o: function outro(local) {
    			transition_out(categorydropdown.$$.fragment, local);
    			current = false;
    		},
    		d: function destroy(detaching) {
    			destroy_component(categorydropdown, detaching);
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_each_block$2.name,
    		type: "each",
    		source: "(30:8) {#each childCategories as child}",
    		ctx
    	});

    	return block;
    }

    function create_fragment$7(ctx) {
    	let div1;
    	let button;
    	let t0;
    	let t1_value = /*category*/ ctx[0].name + "";
    	let t1;
    	let button_class_value;
    	let t2;
    	let div0;
    	let div0_class_value;
    	let current;
    	let mounted;
    	let dispose;

    	function select_block_type(ctx, dirty) {
    		if (!/*expanded*/ ctx[2]) return create_if_block$2;
    		return create_else_block;
    	}

    	let current_block_type = select_block_type(ctx);
    	let if_block = current_block_type(ctx);
    	let each_value = /*childCategories*/ ctx[4];
    	validate_each_argument(each_value);
    	let each_blocks = [];

    	for (let i = 0; i < each_value.length; i += 1) {
    		each_blocks[i] = create_each_block$2(get_each_context$2(ctx, each_value, i));
    	}

    	const out = i => transition_out(each_blocks[i], 1, 1, () => {
    		each_blocks[i] = null;
    	});

    	const block = {
    		c: function create() {
    			div1 = element("div");
    			button = element("button");
    			if_block.c();
    			t0 = space();
    			t1 = text(t1_value);
    			t2 = space();
    			div0 = element("div");

    			for (let i = 0; i < each_blocks.length; i += 1) {
    				each_blocks[i].c();
    			}

    			attr_dev(button, "class", button_class_value = "" + ((/*category*/ ctx[0].id === /*selectedCategoryId*/ ctx[3]
    			? 'marked'
    			: '') + " dd-button" + " svelte-1syk91q"));

    			add_location(button, file$6, 19, 4, 580);
    			attr_dev(div0, "class", div0_class_value = "" + ((/*expanded*/ ctx[2] ? 'expanded' : 'collapsed') + " subcategory" + " svelte-1syk91q"));
    			add_location(div0, file$6, 28, 4, 892);
    			add_location(div1, file$6, 18, 0, 569);
    		},
    		l: function claim(nodes) {
    			throw new Error("options.hydrate only works if the component was compiled with the `hydratable: true` option");
    		},
    		m: function mount(target, anchor) {
    			insert_dev(target, div1, anchor);
    			append_dev(div1, button);
    			if_block.m(button, null);
    			append_dev(button, t0);
    			append_dev(button, t1);
    			append_dev(div1, t2);
    			append_dev(div1, div0);

    			for (let i = 0; i < each_blocks.length; i += 1) {
    				each_blocks[i].m(div0, null);
    			}

    			current = true;

    			if (!mounted) {
    				dispose = listen_dev(button, "click", /*toggleExpandAndSelect*/ ctx[5], false, false, false);
    				mounted = true;
    			}
    		},
    		p: function update(ctx, [dirty]) {
    			if (current_block_type !== (current_block_type = select_block_type(ctx))) {
    				if_block.d(1);
    				if_block = current_block_type(ctx);

    				if (if_block) {
    					if_block.c();
    					if_block.m(button, t0);
    				}
    			}

    			if ((!current || dirty & /*category*/ 1) && t1_value !== (t1_value = /*category*/ ctx[0].name + "")) set_data_dev(t1, t1_value);

    			if (!current || dirty & /*category, selectedCategoryId*/ 9 && button_class_value !== (button_class_value = "" + ((/*category*/ ctx[0].id === /*selectedCategoryId*/ ctx[3]
    			? 'marked'
    			: '') + " dd-button" + " svelte-1syk91q"))) {
    				attr_dev(button, "class", button_class_value);
    			}

    			if (dirty & /*childCategories, allCategories*/ 18) {
    				each_value = /*childCategories*/ ctx[4];
    				validate_each_argument(each_value);
    				let i;

    				for (i = 0; i < each_value.length; i += 1) {
    					const child_ctx = get_each_context$2(ctx, each_value, i);

    					if (each_blocks[i]) {
    						each_blocks[i].p(child_ctx, dirty);
    						transition_in(each_blocks[i], 1);
    					} else {
    						each_blocks[i] = create_each_block$2(child_ctx);
    						each_blocks[i].c();
    						transition_in(each_blocks[i], 1);
    						each_blocks[i].m(div0, null);
    					}
    				}

    				group_outros();

    				for (i = each_value.length; i < each_blocks.length; i += 1) {
    					out(i);
    				}

    				check_outros();
    			}

    			if (!current || dirty & /*expanded*/ 4 && div0_class_value !== (div0_class_value = "" + ((/*expanded*/ ctx[2] ? 'expanded' : 'collapsed') + " subcategory" + " svelte-1syk91q"))) {
    				attr_dev(div0, "class", div0_class_value);
    			}
    		},
    		i: function intro(local) {
    			if (current) return;

    			for (let i = 0; i < each_value.length; i += 1) {
    				transition_in(each_blocks[i]);
    			}

    			current = true;
    		},
    		o: function outro(local) {
    			each_blocks = each_blocks.filter(Boolean);

    			for (let i = 0; i < each_blocks.length; i += 1) {
    				transition_out(each_blocks[i]);
    			}

    			current = false;
    		},
    		d: function destroy(detaching) {
    			if (detaching) detach_dev(div1);
    			if_block.d();
    			destroy_each(each_blocks, detaching);
    			mounted = false;
    			dispose();
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_fragment$7.name,
    		type: "component",
    		source: "",
    		ctx
    	});

    	return block;
    }

    function instance$7($$self, $$props, $$invalidate) {
    	let { $$slots: slots = {}, $$scope } = $$props;
    	validate_slots('CategoryDropdown', slots, []);
    	let { category } = $$props;
    	let { allCategories } = $$props;

    	onMount(() => {
    		selectedCategoryId$.subscribe(x => {
    			$$invalidate(3, selectedCategoryId = x);
    		});
    	});

    	let expanded = true;
    	let selectedCategoryId = '';
    	let childCategories = allCategories.filter(x => x.parentCategoryId === category.id);

    	function toggleExpandAndSelect() {
    		$$invalidate(2, expanded = !expanded);
    		selectedCategoryId$.set(category.id);
    	}

    	$$self.$$.on_mount.push(function () {
    		if (category === undefined && !('category' in $$props || $$self.$$.bound[$$self.$$.props['category']])) {
    			console.warn("<CategoryDropdown> was created without expected prop 'category'");
    		}

    		if (allCategories === undefined && !('allCategories' in $$props || $$self.$$.bound[$$self.$$.props['allCategories']])) {
    			console.warn("<CategoryDropdown> was created without expected prop 'allCategories'");
    		}
    	});

    	const writable_props = ['category', 'allCategories'];

    	Object.keys($$props).forEach(key => {
    		if (!~writable_props.indexOf(key) && key.slice(0, 2) !== '$$' && key !== 'slot') console.warn(`<CategoryDropdown> was created with unknown prop '${key}'`);
    	});

    	$$self.$$set = $$props => {
    		if ('category' in $$props) $$invalidate(0, category = $$props.category);
    		if ('allCategories' in $$props) $$invalidate(1, allCategories = $$props.allCategories);
    	};

    	$$self.$capture_state = () => ({
    		onMount,
    		categoryService,
    		category,
    		allCategories,
    		expanded,
    		selectedCategoryId,
    		childCategories,
    		toggleExpandAndSelect
    	});

    	$$self.$inject_state = $$props => {
    		if ('category' in $$props) $$invalidate(0, category = $$props.category);
    		if ('allCategories' in $$props) $$invalidate(1, allCategories = $$props.allCategories);
    		if ('expanded' in $$props) $$invalidate(2, expanded = $$props.expanded);
    		if ('selectedCategoryId' in $$props) $$invalidate(3, selectedCategoryId = $$props.selectedCategoryId);
    		if ('childCategories' in $$props) $$invalidate(4, childCategories = $$props.childCategories);
    	};

    	if ($$props && "$$inject" in $$props) {
    		$$self.$inject_state($$props.$$inject);
    	}

    	return [
    		category,
    		allCategories,
    		expanded,
    		selectedCategoryId,
    		childCategories,
    		toggleExpandAndSelect
    	];
    }

    class CategoryDropdown extends SvelteComponentDev {
    	constructor(options) {
    		super(options);
    		init(this, options, instance$7, create_fragment$7, safe_not_equal, { category: 0, allCategories: 1 });

    		dispatch_dev("SvelteRegisterComponent", {
    			component: this,
    			tagName: "CategoryDropdown",
    			options,
    			id: create_fragment$7.name
    		});
    	}

    	get category() {
    		throw new Error("<CategoryDropdown>: Props cannot be read directly from the component instance unless compiling with 'accessors: true' or '<svelte:options accessors/>'");
    	}

    	set category(value) {
    		throw new Error("<CategoryDropdown>: Props cannot be set directly on the component instance unless compiling with 'accessors: true' or '<svelte:options accessors/>'");
    	}

    	get allCategories() {
    		throw new Error("<CategoryDropdown>: Props cannot be read directly from the component instance unless compiling with 'accessors: true' or '<svelte:options accessors/>'");
    	}

    	set allCategories(value) {
    		throw new Error("<CategoryDropdown>: Props cannot be set directly on the component instance unless compiling with 'accessors: true' or '<svelte:options accessors/>'");
    	}
    }

    var extendStatics=function(e,t){return extendStatics=Object.setPrototypeOf||{__proto__:[]}instanceof Array&&function(e,t){e.__proto__=t;}||function(e,t){for(var n in t)Object.prototype.hasOwnProperty.call(t,n)&&(e[n]=t[n]);},extendStatics(e,t)};function __extends(e,t){if("function"!=typeof t&&null!==t)throw new TypeError("Class extends value "+String(t)+" is not a constructor or null");function __(){this.constructor=e;}extendStatics(e,t),e.prototype=null===t?Object.create(t):(__.prototype=t.prototype,new __);}var __assign=function(){return __assign=Object.assign||function __assign(e){for(var t,n=1,i=arguments.length;n<i;n++)for(var o in t=arguments[n])Object.prototype.hasOwnProperty.call(t,o)&&(e[o]=t[o]);return e},__assign.apply(this,arguments)};function __awaiter(e,t,n,i){return new(n||(n=Promise))((function(o,r){function fulfilled(e){try{step(i.next(e));}catch(e){r(e);}}function rejected(e){try{step(i.throw(e));}catch(e){r(e);}}function step(e){e.done?o(e.value):function adopt(e){return e instanceof n?e:new n((function(t){t(e);}))}(e.value).then(fulfilled,rejected);}step((i=i.apply(e,t||[])).next());}))}function __generator(e,t){var n,i,o,r,s={label:0,sent:function(){if(1&o[0])throw o[1];return o[1]},trys:[],ops:[]};return r={next:verb(0),throw:verb(1),return:verb(2)},"function"==typeof Symbol&&(r[Symbol.iterator]=function(){return this}),r;function verb(r){return function(a){return function step(r){if(n)throw new TypeError("Generator is already executing.");for(;s;)try{if(n=1,i&&(o=2&r[0]?i.return:r[0]?i.throw||((o=i.return)&&o.call(i),0):i.next)&&!(o=o.call(i,r[1])).done)return o;switch(i=0,o&&(r=[2&r[0],o.value]),r[0]){case 0:case 1:o=r;break;case 4:return s.label++,{value:r[1],done:!1};case 5:s.label++,i=r[1],r=[0];continue;case 7:r=s.ops.pop(),s.trys.pop();continue;default:if(!(o=s.trys,(o=o.length>0&&o[o.length-1])||6!==r[0]&&2!==r[0])){s=0;continue}if(3===r[0]&&(!o||r[1]>o[0]&&r[1]<o[3])){s.label=r[1];break}if(6===r[0]&&s.label<o[1]){s.label=o[1],o=r;break}if(o&&s.label<o[2]){s.label=o[2],s.ops.push(r);break}o[2]&&s.ops.pop(),s.trys.pop();continue}r=t.call(e,s);}catch(e){r=[6,e],i=0;}finally{n=o=0;}if(5&r[0])throw r[1];return {value:r[0]?r[1]:void 0,done:!0}}([r,a])}}}var e,t=function(e){function ClientResponseError(t){var n,i,o,r,s=this;return (s=e.call(this,"ClientResponseError")||this).url="",s.status=0,s.response={},s.isAbort=!1,s.originalError=null,Object.setPrototypeOf(s,ClientResponseError.prototype),t instanceof ClientResponseError||(s.originalError=t),null!==t&&"object"==typeof t&&(s.url="string"==typeof t.url?t.url:"",s.status="number"==typeof t.status?t.status:0,s.response=null!==t.data&&"object"==typeof t.data?t.data:{},s.isAbort=!!t.isAbort),"undefined"!=typeof DOMException&&t instanceof DOMException&&(s.isAbort=!0),s.name="ClientResponseError "+s.status,s.message=null===(n=s.response)||void 0===n?void 0:n.message,s.message||(s.isAbort?s.message="The request was autocancelled. You can find more info in https://github.com/pocketbase/js-sdk#auto-cancellation.":(null===(r=null===(o=null===(i=s.originalError)||void 0===i?void 0:i.cause)||void 0===o?void 0:o.message)||void 0===r?void 0:r.includes("ECONNREFUSED ::1"))?s.message="Failed to connect to the PocketBase server. Try changing the SDK URL from localhost to 127.0.0.1 (https://github.com/pocketbase/js-sdk/issues/21).":s.message="Something went wrong while processing your request."),s}return __extends(ClientResponseError,e),Object.defineProperty(ClientResponseError.prototype,"data",{get:function(){return this.response},enumerable:!1,configurable:!0}),ClientResponseError.prototype.toJSON=function(){return __assign({},this)},ClientResponseError}(Error),n=/^[\u0009\u0020-\u007e\u0080-\u00ff]+$/;function cookieSerialize(e,t,i){var o=Object.assign({},i||{}),r=o.encode||defaultEncode;if(!n.test(e))throw new TypeError("argument name is invalid");var s=r(t);if(s&&!n.test(s))throw new TypeError("argument val is invalid");var a=e+"="+s;if(null!=o.maxAge){var c=o.maxAge-0;if(isNaN(c)||!isFinite(c))throw new TypeError("option maxAge is invalid");a+="; Max-Age="+Math.floor(c);}if(o.domain){if(!n.test(o.domain))throw new TypeError("option domain is invalid");a+="; Domain="+o.domain;}if(o.path){if(!n.test(o.path))throw new TypeError("option path is invalid");a+="; Path="+o.path;}if(o.expires){if(!function isDate(e){return "[object Date]"===Object.prototype.toString.call(e)||e instanceof Date}(o.expires)||isNaN(o.expires.valueOf()))throw new TypeError("option expires is invalid");a+="; Expires="+o.expires.toUTCString();}if(o.httpOnly&&(a+="; HttpOnly"),o.secure&&(a+="; Secure"),o.priority)switch("string"==typeof o.priority?o.priority.toLowerCase():o.priority){case"low":a+="; Priority=Low";break;case"medium":a+="; Priority=Medium";break;case"high":a+="; Priority=High";break;default:throw new TypeError("option priority is invalid")}if(o.sameSite)switch("string"==typeof o.sameSite?o.sameSite.toLowerCase():o.sameSite){case!0:a+="; SameSite=Strict";break;case"lax":a+="; SameSite=Lax";break;case"strict":a+="; SameSite=Strict";break;case"none":a+="; SameSite=None";break;default:throw new TypeError("option sameSite is invalid")}return a}function defaultDecode(e){return -1!==e.indexOf("%")?decodeURIComponent(e):e}function defaultEncode(e){return encodeURIComponent(e)}function getTokenPayload(t){if(t)try{var n=decodeURIComponent(e(t.split(".")[1]).split("").map((function(e){return "%"+("00"+e.charCodeAt(0).toString(16)).slice(-2)})).join(""));return JSON.parse(n)||{}}catch(e){}return {}}e="function"==typeof atob?atob:function(e){var t=String(e).replace(/=+$/,"");if(t.length%4==1)throw new Error("'atob' failed: The string to be decoded is not correctly encoded.");for(var n,i,o=0,r=0,s="";i=t.charAt(r++);~i&&(n=o%4?64*n+i:i,o++%4)?s+=String.fromCharCode(255&n>>(-2*o&6)):0)i="ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=".indexOf(i);return s};var i=function(){function BaseModel(e){void 0===e&&(e={}),this.load(e||{});}return BaseModel.prototype.load=function(e){for(var t=0,n=Object.entries(e);t<n.length;t++){var i=n[t],o=i[0],r=i[1];this[o]=r;}this.id=void 0!==e.id?e.id:"",this.created=void 0!==e.created?e.created:"",this.updated=void 0!==e.updated?e.updated:"";},Object.defineProperty(BaseModel.prototype,"isNew",{get:function(){return !this.id},enumerable:!1,configurable:!0}),BaseModel.prototype.clone=function(){var e="function"==typeof structuredClone?structuredClone(this):JSON.parse(JSON.stringify(this));return new this.constructor(e)},BaseModel.prototype.export=function(){return Object.assign({},this)},BaseModel}(),o=function(e){function Record(){return null!==e&&e.apply(this,arguments)||this}return __extends(Record,e),Record.prototype.load=function(t){e.prototype.load.call(this,t),this.collectionId="string"==typeof t.collectionId?t.collectionId:"",this.collectionName="string"==typeof t.collectionName?t.collectionName:"",this.loadExpand(t.expand);},Record.prototype.loadExpand=function(e){for(var t in e=e||{},this.expand={},e)Array.isArray(e[t])?this.expand[t]=e[t].map((function(e){return new Record(e||{})})):this.expand[t]=new Record(e[t]||{});},Record}(i),r=function(e){function Admin(){return null!==e&&e.apply(this,arguments)||this}return __extends(Admin,e),Admin.prototype.load=function(t){e.prototype.load.call(this,t),this.avatar="number"==typeof t.avatar?t.avatar:0,this.email="string"==typeof t.email?t.email:"";},Admin}(i),s=function(){function BaseAuthStore(){this.baseToken="",this.baseModel=null,this._onChangeCallbacks=[];}return Object.defineProperty(BaseAuthStore.prototype,"token",{get:function(){return this.baseToken},enumerable:!1,configurable:!0}),Object.defineProperty(BaseAuthStore.prototype,"model",{get:function(){return this.baseModel},enumerable:!1,configurable:!0}),Object.defineProperty(BaseAuthStore.prototype,"isValid",{get:function(){return !function isTokenExpired(e,t){void 0===t&&(t=0);var n=getTokenPayload(e);return !(Object.keys(n).length>0&&(!n.exp||n.exp-t>Date.now()/1e3))}(this.token)},enumerable:!1,configurable:!0}),BaseAuthStore.prototype.save=function(e,t){this.baseToken=e||"",this.baseModel=null!==t&&"object"==typeof t?void 0!==t.collectionId?new o(t):new r(t):null,this.triggerChange();},BaseAuthStore.prototype.clear=function(){this.baseToken="",this.baseModel=null,this.triggerChange();},BaseAuthStore.prototype.loadFromCookie=function(e,t){void 0===t&&(t="pb_auth");var n=function cookieParse(e,t){var n={};if("string"!=typeof e)return n;for(var i=Object.assign({},t||{}).decode||defaultDecode,o=0;o<e.length;){var r=e.indexOf("=",o);if(-1===r)break;var s=e.indexOf(";",o);if(-1===s)s=e.length;else if(s<r){o=e.lastIndexOf(";",r-1)+1;continue}var a=e.slice(o,r).trim();if(void 0===n[a]){var c=e.slice(r+1,s).trim();34===c.charCodeAt(0)&&(c=c.slice(1,-1));try{n[a]=i(c);}catch(e){n[a]=c;}}o=s+1;}return n}(e||"")[t]||"",i={};try{(null===typeof(i=JSON.parse(n))||"object"!=typeof i||Array.isArray(i))&&(i={});}catch(e){}this.save(i.token||"",i.model||null);},BaseAuthStore.prototype.exportToCookie=function(e,t){var n,i,r;void 0===t&&(t="pb_auth");var s={secure:!0,sameSite:!0,httpOnly:!0,path:"/"},a=getTokenPayload(this.token);(null==a?void 0:a.exp)?s.expires=new Date(1e3*a.exp):s.expires=new Date("1970-01-01"),e=Object.assign({},s,e);var c={token:this.token,model:(null===(n=this.model)||void 0===n?void 0:n.export())||null},u=cookieSerialize(t,JSON.stringify(c),e),l="undefined"!=typeof Blob?new Blob([u]).size:u.length;return c.model&&l>4096&&(c.model={id:null===(i=null==c?void 0:c.model)||void 0===i?void 0:i.id,email:null===(r=null==c?void 0:c.model)||void 0===r?void 0:r.email},this.model instanceof o&&(c.model.username=this.model.username,c.model.verified=this.model.verified,c.model.collectionId=this.model.collectionId),u=cookieSerialize(t,JSON.stringify(c),e)),u},BaseAuthStore.prototype.onChange=function(e,t){var n=this;return void 0===t&&(t=!1),this._onChangeCallbacks.push(e),t&&e(this.token,this.model),function(){for(var t=n._onChangeCallbacks.length-1;t>=0;t--)if(n._onChangeCallbacks[t]==e)return delete n._onChangeCallbacks[t],void n._onChangeCallbacks.splice(t,1)}},BaseAuthStore.prototype.triggerChange=function(){for(var e=0,t=this._onChangeCallbacks;e<t.length;e++){var n=t[e];n&&n(this.token,this.model);}},BaseAuthStore}(),a=function(e){function LocalAuthStore(t){void 0===t&&(t="pocketbase_auth");var n=e.call(this)||this;return n.storageFallback={},n.storageKey=t,n}return __extends(LocalAuthStore,e),Object.defineProperty(LocalAuthStore.prototype,"token",{get:function(){return (this._storageGet(this.storageKey)||{}).token||""},enumerable:!1,configurable:!0}),Object.defineProperty(LocalAuthStore.prototype,"model",{get:function(){var e,t=this._storageGet(this.storageKey)||{};return null===t||"object"!=typeof t||null===t.model||"object"!=typeof t.model?null:void 0===(null===(e=t.model)||void 0===e?void 0:e.collectionId)?new r(t.model):new o(t.model)},enumerable:!1,configurable:!0}),LocalAuthStore.prototype.save=function(t,n){this._storageSet(this.storageKey,{token:t,model:n}),e.prototype.save.call(this,t,n);},LocalAuthStore.prototype.clear=function(){this._storageRemove(this.storageKey),e.prototype.clear.call(this);},LocalAuthStore.prototype._storageGet=function(e){if("undefined"!=typeof window&&(null===window||void 0===window?void 0:window.localStorage)){var t=window.localStorage.getItem(e)||"";try{return JSON.parse(t)}catch(e){return t}}return this.storageFallback[e]},LocalAuthStore.prototype._storageSet=function(e,t){if("undefined"!=typeof window&&(null===window||void 0===window?void 0:window.localStorage)){var n=t;"string"!=typeof t&&(n=JSON.stringify(t)),window.localStorage.setItem(e,n);}else this.storageFallback[e]=t;},LocalAuthStore.prototype._storageRemove=function(e){var t;"undefined"!=typeof window&&(null===window||void 0===window?void 0:window.localStorage)&&(null===(t=window.localStorage)||void 0===t||t.removeItem(e)),delete this.storageFallback[e];},LocalAuthStore}(s),c=function c(e){this.client=e;},u=function(e){function SettingsService(){return null!==e&&e.apply(this,arguments)||this}return __extends(SettingsService,e),SettingsService.prototype.getAll=function(e){return void 0===e&&(e={}),this.client.send("/api/settings",{method:"GET",params:e}).then((function(e){return e||{}}))},SettingsService.prototype.update=function(e,t){return void 0===e&&(e={}),void 0===t&&(t={}),this.client.send("/api/settings",{method:"PATCH",params:t,body:e}).then((function(e){return e||{}}))},SettingsService.prototype.testS3=function(e){return void 0===e&&(e={}),this.client.send("/api/settings/test/s3",{method:"POST",params:e}).then((function(){return !0}))},SettingsService.prototype.testEmail=function(e,t,n){void 0===n&&(n={});var i={email:e,template:t};return this.client.send("/api/settings/test/email",{method:"POST",params:n,body:i}).then((function(){return !0}))},SettingsService}(c),l=function l(e,t,n,i,o){this.page=e>0?e:1,this.perPage=t>=0?t:0,this.totalItems=n>=0?n:0,this.totalPages=i>=0?i:0,this.items=o||[];},d=function(e){function CrudService(){return null!==e&&e.apply(this,arguments)||this}return __extends(CrudService,e),CrudService.prototype.getFullList=function(e,t){if("number"==typeof e)return this._getFullList(this.baseCrudPath,e,t);var n=Object.assign({},e,t);return this._getFullList(this.baseCrudPath,n.batch||200,n)},CrudService.prototype.getList=function(e,t,n){return void 0===e&&(e=1),void 0===t&&(t=30),void 0===n&&(n={}),this._getList(this.baseCrudPath,e,t,n)},CrudService.prototype.getFirstListItem=function(e,t){return void 0===t&&(t={}),this._getFirstListItem(this.baseCrudPath,e,t)},CrudService.prototype.getOne=function(e,t){return void 0===t&&(t={}),this._getOne(this.baseCrudPath,e,t)},CrudService.prototype.create=function(e,t){return void 0===e&&(e={}),void 0===t&&(t={}),this._create(this.baseCrudPath,e,t)},CrudService.prototype.update=function(e,t,n){return void 0===t&&(t={}),void 0===n&&(n={}),this._update(this.baseCrudPath,e,t,n)},CrudService.prototype.delete=function(e,t){return void 0===t&&(t={}),this._delete(this.baseCrudPath,e,t)},CrudService}(function(e){function BaseCrudService(){return null!==e&&e.apply(this,arguments)||this}return __extends(BaseCrudService,e),BaseCrudService.prototype._getFullList=function(e,t,n){var i=this;void 0===t&&(t=200),void 0===n&&(n={});var o=[],request=function(r){return __awaiter(i,void 0,void 0,(function(){return __generator(this,(function(i){return [2,this._getList(e,r,t||200,n).then((function(e){var t=e,n=t.items,i=t.totalItems;return o=o.concat(n),n.length&&i>o.length?request(r+1):o}))]}))}))};return request(1)},BaseCrudService.prototype._getList=function(e,t,n,i){var o=this;return void 0===t&&(t=1),void 0===n&&(n=30),void 0===i&&(i={}),i=Object.assign({page:t,perPage:n},i),this.client.send(e,{method:"GET",params:i}).then((function(e){var t=[];if(null==e?void 0:e.items){e.items=e.items||[];for(var n=0,i=e.items;n<i.length;n++){var r=i[n];t.push(o.decode(r));}}return new l((null==e?void 0:e.page)||1,(null==e?void 0:e.perPage)||0,(null==e?void 0:e.totalItems)||0,(null==e?void 0:e.totalPages)||0,t)}))},BaseCrudService.prototype._getOne=function(e,t,n){var i=this;return void 0===n&&(n={}),this.client.send(e+"/"+encodeURIComponent(t),{method:"GET",params:n}).then((function(e){return i.decode(e)}))},BaseCrudService.prototype._getFirstListItem=function(e,n,i){return void 0===i&&(i={}),i=Object.assign({filter:n,$cancelKey:"one_by_filter_"+e+"_"+n},i),this._getList(e,1,1,i).then((function(e){var n;if(!(null===(n=null==e?void 0:e.items)||void 0===n?void 0:n.length))throw new t({status:404,data:{code:404,message:"The requested resource wasn't found.",data:{}}});return e.items[0]}))},BaseCrudService.prototype._create=function(e,t,n){var i=this;return void 0===t&&(t={}),void 0===n&&(n={}),this.client.send(e,{method:"POST",params:n,body:t}).then((function(e){return i.decode(e)}))},BaseCrudService.prototype._update=function(e,t,n,i){var o=this;return void 0===n&&(n={}),void 0===i&&(i={}),this.client.send(e+"/"+encodeURIComponent(t),{method:"PATCH",params:i,body:n}).then((function(e){return o.decode(e)}))},BaseCrudService.prototype._delete=function(e,t,n){return void 0===n&&(n={}),this.client.send(e+"/"+encodeURIComponent(t),{method:"DELETE",params:n}).then((function(){return !0}))},BaseCrudService}(c)),h=function(e){function AdminService(){return null!==e&&e.apply(this,arguments)||this}return __extends(AdminService,e),AdminService.prototype.decode=function(e){return new r(e)},Object.defineProperty(AdminService.prototype,"baseCrudPath",{get:function(){return "/api/admins"},enumerable:!1,configurable:!0}),AdminService.prototype.update=function(t,n,i){var o=this;return void 0===n&&(n={}),void 0===i&&(i={}),e.prototype.update.call(this,t,n,i).then((function(e){var t,n;return o.client.authStore.model&&void 0===(null===(t=o.client.authStore.model)||void 0===t?void 0:t.collectionId)&&(null===(n=o.client.authStore.model)||void 0===n?void 0:n.id)===(null==e?void 0:e.id)&&o.client.authStore.save(o.client.authStore.token,e),e}))},AdminService.prototype.delete=function(t,n){var i=this;return void 0===n&&(n={}),e.prototype.delete.call(this,t,n).then((function(e){var n,o;return e&&i.client.authStore.model&&void 0===(null===(n=i.client.authStore.model)||void 0===n?void 0:n.collectionId)&&(null===(o=i.client.authStore.model)||void 0===o?void 0:o.id)===t&&i.client.authStore.clear(),e}))},AdminService.prototype.authResponse=function(e){var t=this.decode((null==e?void 0:e.admin)||{});return (null==e?void 0:e.token)&&(null==e?void 0:e.admin)&&this.client.authStore.save(e.token,t),Object.assign({},e,{token:(null==e?void 0:e.token)||"",admin:t})},AdminService.prototype.authWithPassword=function(e,t,n,i){return void 0===n&&(n={}),void 0===i&&(i={}),n=Object.assign({identity:e,password:t},n),this.client.send(this.baseCrudPath+"/auth-with-password",{method:"POST",params:i,body:n}).then(this.authResponse.bind(this))},AdminService.prototype.authRefresh=function(e,t){return void 0===e&&(e={}),void 0===t&&(t={}),this.client.send(this.baseCrudPath+"/auth-refresh",{method:"POST",params:t,body:e}).then(this.authResponse.bind(this))},AdminService.prototype.requestPasswordReset=function(e,t,n){return void 0===t&&(t={}),void 0===n&&(n={}),t=Object.assign({email:e},t),this.client.send(this.baseCrudPath+"/request-password-reset",{method:"POST",params:n,body:t}).then((function(){return !0}))},AdminService.prototype.confirmPasswordReset=function(e,t,n,i,o){return void 0===i&&(i={}),void 0===o&&(o={}),i=Object.assign({token:e,password:t,passwordConfirm:n},i),this.client.send(this.baseCrudPath+"/confirm-password-reset",{method:"POST",params:o,body:i}).then((function(){return !0}))},AdminService}(d),p=function(e){function ExternalAuth(){return null!==e&&e.apply(this,arguments)||this}return __extends(ExternalAuth,e),ExternalAuth.prototype.load=function(t){e.prototype.load.call(this,t),this.recordId="string"==typeof t.recordId?t.recordId:"",this.collectionId="string"==typeof t.collectionId?t.collectionId:"",this.provider="string"==typeof t.provider?t.provider:"",this.providerId="string"==typeof t.providerId?t.providerId:"";},ExternalAuth}(i),v=function(e){function RecordService(t,n){var i=e.call(this,t)||this;return i.collectionIdOrName=n,i}return __extends(RecordService,e),RecordService.prototype.decode=function(e){return new o(e)},Object.defineProperty(RecordService.prototype,"baseCrudPath",{get:function(){return this.baseCollectionPath+"/records"},enumerable:!1,configurable:!0}),Object.defineProperty(RecordService.prototype,"baseCollectionPath",{get:function(){return "/api/collections/"+encodeURIComponent(this.collectionIdOrName)},enumerable:!1,configurable:!0}),RecordService.prototype.subscribeOne=function(e,t){return __awaiter(this,void 0,void 0,(function(){return __generator(this,(function(n){return console.warn("PocketBase: subscribeOne(recordId, callback) is deprecated. Please replace it with subscribe(recordId, callback)."),[2,this.client.realtime.subscribe(this.collectionIdOrName+"/"+e,t)]}))}))},RecordService.prototype.subscribe=function(e,t){return __awaiter(this,void 0,void 0,(function(){var n;return __generator(this,(function(i){if("function"==typeof e)return console.warn("PocketBase: subscribe(callback) is deprecated. Please replace it with subscribe('*', callback)."),[2,this.client.realtime.subscribe(this.collectionIdOrName,e)];if(!t)throw new Error("Missing subscription callback.");if(""===e)throw new Error("Missing topic.");return n=this.collectionIdOrName,"*"!==e&&(n+="/"+e),[2,this.client.realtime.subscribe(n,t)]}))}))},RecordService.prototype.unsubscribe=function(e){return __awaiter(this,void 0,void 0,(function(){return __generator(this,(function(t){return "*"===e?[2,this.client.realtime.unsubscribe(this.collectionIdOrName)]:e?[2,this.client.realtime.unsubscribe(this.collectionIdOrName+"/"+e)]:[2,this.client.realtime.unsubscribeByPrefix(this.collectionIdOrName)]}))}))},RecordService.prototype.getFullList=function(t,n){if("number"==typeof t)return e.prototype.getFullList.call(this,t,n);var i=Object.assign({},t,n);return e.prototype.getFullList.call(this,i)},RecordService.prototype.getList=function(t,n,i){return void 0===t&&(t=1),void 0===n&&(n=30),void 0===i&&(i={}),e.prototype.getList.call(this,t,n,i)},RecordService.prototype.getFirstListItem=function(t,n){return void 0===n&&(n={}),e.prototype.getFirstListItem.call(this,t,n)},RecordService.prototype.getOne=function(t,n){return void 0===n&&(n={}),e.prototype.getOne.call(this,t,n)},RecordService.prototype.create=function(t,n){return void 0===t&&(t={}),void 0===n&&(n={}),e.prototype.create.call(this,t,n)},RecordService.prototype.update=function(t,n,i){var o=this;return void 0===n&&(n={}),void 0===i&&(i={}),e.prototype.update.call(this,t,n,i).then((function(e){var t,n,i;return (null===(t=o.client.authStore.model)||void 0===t?void 0:t.id)!==(null==e?void 0:e.id)||(null===(n=o.client.authStore.model)||void 0===n?void 0:n.collectionId)!==o.collectionIdOrName&&(null===(i=o.client.authStore.model)||void 0===i?void 0:i.collectionName)!==o.collectionIdOrName||o.client.authStore.save(o.client.authStore.token,e),e}))},RecordService.prototype.delete=function(t,n){var i=this;return void 0===n&&(n={}),e.prototype.delete.call(this,t,n).then((function(e){var n,o,r;return !e||(null===(n=i.client.authStore.model)||void 0===n?void 0:n.id)!==t||(null===(o=i.client.authStore.model)||void 0===o?void 0:o.collectionId)!==i.collectionIdOrName&&(null===(r=i.client.authStore.model)||void 0===r?void 0:r.collectionName)!==i.collectionIdOrName||i.client.authStore.clear(),e}))},RecordService.prototype.authResponse=function(e){var t=this.decode((null==e?void 0:e.record)||{});return this.client.authStore.save(null==e?void 0:e.token,t),Object.assign({},e,{token:(null==e?void 0:e.token)||"",record:t})},RecordService.prototype.listAuthMethods=function(e){return void 0===e&&(e={}),this.client.send(this.baseCollectionPath+"/auth-methods",{method:"GET",params:e}).then((function(e){return Object.assign({},e,{usernamePassword:!!(null==e?void 0:e.usernamePassword),emailPassword:!!(null==e?void 0:e.emailPassword),authProviders:Array.isArray(null==e?void 0:e.authProviders)?null==e?void 0:e.authProviders:[]})}))},RecordService.prototype.authWithPassword=function(e,t,n,i){var o=this;return void 0===n&&(n={}),void 0===i&&(i={}),n=Object.assign({identity:e,password:t},n),this.client.send(this.baseCollectionPath+"/auth-with-password",{method:"POST",params:i,body:n}).then((function(e){return o.authResponse(e)}))},RecordService.prototype.authWithOAuth2=function(e,t,n,i,o,r,s){var a=this;return void 0===o&&(o={}),void 0===r&&(r={}),void 0===s&&(s={}),r=Object.assign({provider:e,code:t,codeVerifier:n,redirectUrl:i,createData:o},r),this.client.send(this.baseCollectionPath+"/auth-with-oauth2",{method:"POST",params:s,body:r}).then((function(e){return a.authResponse(e)}))},RecordService.prototype.authRefresh=function(e,t){var n=this;return void 0===e&&(e={}),void 0===t&&(t={}),this.client.send(this.baseCollectionPath+"/auth-refresh",{method:"POST",params:t,body:e}).then((function(e){return n.authResponse(e)}))},RecordService.prototype.requestPasswordReset=function(e,t,n){return void 0===t&&(t={}),void 0===n&&(n={}),t=Object.assign({email:e},t),this.client.send(this.baseCollectionPath+"/request-password-reset",{method:"POST",params:n,body:t}).then((function(){return !0}))},RecordService.prototype.confirmPasswordReset=function(e,t,n,i,o){return void 0===i&&(i={}),void 0===o&&(o={}),i=Object.assign({token:e,password:t,passwordConfirm:n},i),this.client.send(this.baseCollectionPath+"/confirm-password-reset",{method:"POST",params:o,body:i}).then((function(){return !0}))},RecordService.prototype.requestVerification=function(e,t,n){return void 0===t&&(t={}),void 0===n&&(n={}),t=Object.assign({email:e},t),this.client.send(this.baseCollectionPath+"/request-verification",{method:"POST",params:n,body:t}).then((function(){return !0}))},RecordService.prototype.confirmVerification=function(e,t,n){return void 0===t&&(t={}),void 0===n&&(n={}),t=Object.assign({token:e},t),this.client.send(this.baseCollectionPath+"/confirm-verification",{method:"POST",params:n,body:t}).then((function(){return !0}))},RecordService.prototype.requestEmailChange=function(e,t,n){return void 0===t&&(t={}),void 0===n&&(n={}),t=Object.assign({newEmail:e},t),this.client.send(this.baseCollectionPath+"/request-email-change",{method:"POST",params:n,body:t}).then((function(){return !0}))},RecordService.prototype.confirmEmailChange=function(e,t,n,i){return void 0===n&&(n={}),void 0===i&&(i={}),n=Object.assign({token:e,password:t},n),this.client.send(this.baseCollectionPath+"/confirm-email-change",{method:"POST",params:i,body:n}).then((function(){return !0}))},RecordService.prototype.listExternalAuths=function(e,t){return void 0===t&&(t={}),this.client.send(this.baseCrudPath+"/"+encodeURIComponent(e)+"/external-auths",{method:"GET",params:t}).then((function(e){var t=[];if(Array.isArray(e))for(var n=0,i=e;n<i.length;n++){var o=i[n];t.push(new p(o));}return t}))},RecordService.prototype.unlinkExternalAuth=function(e,t,n){return void 0===n&&(n={}),this.client.send(this.baseCrudPath+"/"+encodeURIComponent(e)+"/external-auths/"+encodeURIComponent(t),{method:"DELETE",params:n}).then((function(){return !0}))},RecordService}(d),f=function(){function SchemaField(e){void 0===e&&(e={}),this.load(e||{});}return SchemaField.prototype.load=function(e){this.id=void 0!==e.id?e.id:"",this.name=void 0!==e.name?e.name:"",this.type=void 0!==e.type?e.type:"text",this.system=!!e.system,this.required=!!e.required,this.unique=!!e.unique,this.options="object"==typeof e.options&&null!==e.options?e.options:{};},SchemaField}(),m=function(e){function Collection(){return null!==e&&e.apply(this,arguments)||this}return __extends(Collection,e),Collection.prototype.load=function(t){e.prototype.load.call(this,t),this.system=!!t.system,this.name="string"==typeof t.name?t.name:"",this.type="string"==typeof t.type?t.type:"base",this.options=void 0!==t.options?t.options:{},this.listRule="string"==typeof t.listRule?t.listRule:null,this.viewRule="string"==typeof t.viewRule?t.viewRule:null,this.createRule="string"==typeof t.createRule?t.createRule:null,this.updateRule="string"==typeof t.updateRule?t.updateRule:null,this.deleteRule="string"==typeof t.deleteRule?t.deleteRule:null,t.schema=Array.isArray(t.schema)?t.schema:[],this.schema=[];for(var n=0,i=t.schema;n<i.length;n++){var o=i[n];this.schema.push(new f(o));}},Object.defineProperty(Collection.prototype,"isBase",{get:function(){return "base"===this.type},enumerable:!1,configurable:!0}),Object.defineProperty(Collection.prototype,"isAuth",{get:function(){return "auth"===this.type},enumerable:!1,configurable:!0}),Object.defineProperty(Collection.prototype,"isView",{get:function(){return "view"===this.type},enumerable:!1,configurable:!0}),Collection}(i),b=function(e){function CollectionService(){return null!==e&&e.apply(this,arguments)||this}return __extends(CollectionService,e),CollectionService.prototype.decode=function(e){return new m(e)},Object.defineProperty(CollectionService.prototype,"baseCrudPath",{get:function(){return "/api/collections"},enumerable:!1,configurable:!0}),CollectionService.prototype.import=function(e,t,n){return void 0===t&&(t=!1),void 0===n&&(n={}),__awaiter(this,void 0,void 0,(function(){return __generator(this,(function(i){return [2,this.client.send(this.baseCrudPath+"/import",{method:"PUT",params:n,body:{collections:e,deleteMissing:t}}).then((function(){return !0}))]}))}))},CollectionService}(d),y=function(e){function LogRequest(){return null!==e&&e.apply(this,arguments)||this}return __extends(LogRequest,e),LogRequest.prototype.load=function(t){e.prototype.load.call(this,t),t.remoteIp=t.remoteIp||t.ip,this.url="string"==typeof t.url?t.url:"",this.method="string"==typeof t.method?t.method:"GET",this.status="number"==typeof t.status?t.status:200,this.auth="string"==typeof t.auth?t.auth:"guest",this.remoteIp="string"==typeof t.remoteIp?t.remoteIp:"",this.userIp="string"==typeof t.userIp?t.userIp:"",this.referer="string"==typeof t.referer?t.referer:"",this.userAgent="string"==typeof t.userAgent?t.userAgent:"",this.meta="object"==typeof t.meta&&null!==t.meta?t.meta:{};},LogRequest}(i),g=function(e){function LogService(){return null!==e&&e.apply(this,arguments)||this}return __extends(LogService,e),LogService.prototype.getRequestsList=function(e,t,n){return void 0===e&&(e=1),void 0===t&&(t=30),void 0===n&&(n={}),n=Object.assign({page:e,perPage:t},n),this.client.send("/api/logs/requests",{method:"GET",params:n}).then((function(e){var t=[];if(null==e?void 0:e.items){e.items=(null==e?void 0:e.items)||[];for(var n=0,i=e.items;n<i.length;n++){var o=i[n];t.push(new y(o));}}return new l((null==e?void 0:e.page)||1,(null==e?void 0:e.perPage)||0,(null==e?void 0:e.totalItems)||0,(null==e?void 0:e.totalPages)||0,t)}))},LogService.prototype.getRequest=function(e,t){return void 0===t&&(t={}),this.client.send("/api/logs/requests/"+encodeURIComponent(e),{method:"GET",params:t}).then((function(e){return new y(e)}))},LogService.prototype.getRequestsStats=function(e){return void 0===e&&(e={}),this.client.send("/api/logs/requests/stats",{method:"GET",params:e}).then((function(e){return e}))},LogService}(c),S=function(e){function RealtimeService(){var t=null!==e&&e.apply(this,arguments)||this;return t.clientId="",t.eventSource=null,t.subscriptions={},t.lastSentTopics=[],t.maxConnectTimeout=15e3,t.reconnectAttempts=0,t.maxReconnectAttempts=1/0,t.predefinedReconnectIntervals=[200,300,500,1e3,1200,1500,2e3],t.pendingConnects=[],t}return __extends(RealtimeService,e),Object.defineProperty(RealtimeService.prototype,"isConnected",{get:function(){return !!this.eventSource&&!!this.clientId&&!this.pendingConnects.length},enumerable:!1,configurable:!0}),RealtimeService.prototype.subscribe=function(e,t){var n;return __awaiter(this,void 0,void 0,(function(){var i,o=this;return __generator(this,(function(r){switch(r.label){case 0:if(!e)throw new Error("topic must be set.");return i=function(e){var n,i=e;try{n=JSON.parse(null==i?void 0:i.data);}catch(e){}t(n||{});},this.subscriptions[e]||(this.subscriptions[e]=[]),this.subscriptions[e].push(i),this.isConnected?[3,2]:[4,this.connect()];case 1:return r.sent(),[3,5];case 2:return 1!==this.subscriptions[e].length?[3,4]:[4,this.submitSubscriptions()];case 3:return r.sent(),[3,5];case 4:null===(n=this.eventSource)||void 0===n||n.addEventListener(e,i),r.label=5;case 5:return [2,function(){return __awaiter(o,void 0,void 0,(function(){return __generator(this,(function(t){return [2,this.unsubscribeByTopicAndListener(e,i)]}))}))}]}}))}))},RealtimeService.prototype.unsubscribe=function(e){var t;return __awaiter(this,void 0,void 0,(function(){var n,i,o;return __generator(this,(function(r){switch(r.label){case 0:if(!this.hasSubscriptionListeners(e))return [2];if(e){for(n=0,i=this.subscriptions[e];n<i.length;n++)o=i[n],null===(t=this.eventSource)||void 0===t||t.removeEventListener(e,o);delete this.subscriptions[e];}else this.subscriptions={};return this.hasSubscriptionListeners()?[3,1]:(this.disconnect(),[3,3]);case 1:return this.hasSubscriptionListeners(e)?[3,3]:[4,this.submitSubscriptions()];case 2:r.sent(),r.label=3;case 3:return [2]}}))}))},RealtimeService.prototype.unsubscribeByPrefix=function(e){var t;return __awaiter(this,void 0,void 0,(function(){var n,i,o,r,s;return __generator(this,(function(a){switch(a.label){case 0:for(i in n=!1,this.subscriptions)if(i.startsWith(e)){for(n=!0,o=0,r=this.subscriptions[i];o<r.length;o++)s=r[o],null===(t=this.eventSource)||void 0===t||t.removeEventListener(i,s);delete this.subscriptions[i];}return n?this.hasSubscriptionListeners()?[4,this.submitSubscriptions()]:[3,2]:[2];case 1:return a.sent(),[3,3];case 2:this.disconnect(),a.label=3;case 3:return [2]}}))}))},RealtimeService.prototype.unsubscribeByTopicAndListener=function(e,t){var n;return __awaiter(this,void 0,void 0,(function(){var i,o;return __generator(this,(function(r){switch(r.label){case 0:if(!Array.isArray(this.subscriptions[e])||!this.subscriptions[e].length)return [2];for(i=!1,o=this.subscriptions[e].length-1;o>=0;o--)this.subscriptions[e][o]===t&&(i=!0,delete this.subscriptions[e][o],this.subscriptions[e].splice(o,1),null===(n=this.eventSource)||void 0===n||n.removeEventListener(e,t));return i?(this.subscriptions[e].length||delete this.subscriptions[e],this.hasSubscriptionListeners()?[3,1]:(this.disconnect(),[3,3])):[2];case 1:return this.hasSubscriptionListeners(e)?[3,3]:[4,this.submitSubscriptions()];case 2:r.sent(),r.label=3;case 3:return [2]}}))}))},RealtimeService.prototype.hasSubscriptionListeners=function(e){var t,n;if(this.subscriptions=this.subscriptions||{},e)return !!(null===(t=this.subscriptions[e])||void 0===t?void 0:t.length);for(var i in this.subscriptions)if(null===(n=this.subscriptions[i])||void 0===n?void 0:n.length)return !0;return !1},RealtimeService.prototype.submitSubscriptions=function(){return __awaiter(this,void 0,void 0,(function(){return __generator(this,(function(e){return this.clientId?(this.addAllSubscriptionListeners(),this.lastSentTopics=this.getNonEmptySubscriptionTopics(),[2,this.client.send("/api/realtime",{method:"POST",body:{clientId:this.clientId,subscriptions:this.lastSentTopics},params:{$cancelKey:"realtime_"+this.clientId}}).catch((function(e){if(!(null==e?void 0:e.isAbort))throw e}))]):[2]}))}))},RealtimeService.prototype.getNonEmptySubscriptionTopics=function(){var e=[];for(var t in this.subscriptions)this.subscriptions[t].length&&e.push(t);return e},RealtimeService.prototype.addAllSubscriptionListeners=function(){if(this.eventSource)for(var e in this.removeAllSubscriptionListeners(),this.subscriptions)for(var t=0,n=this.subscriptions[e];t<n.length;t++){var i=n[t];this.eventSource.addEventListener(e,i);}},RealtimeService.prototype.removeAllSubscriptionListeners=function(){if(this.eventSource)for(var e in this.subscriptions)for(var t=0,n=this.subscriptions[e];t<n.length;t++){var i=n[t];this.eventSource.removeEventListener(e,i);}},RealtimeService.prototype.connect=function(){return __awaiter(this,void 0,void 0,(function(){var e=this;return __generator(this,(function(t){return this.reconnectAttempts>0?[2]:[2,new Promise((function(t,n){e.pendingConnects.push({resolve:t,reject:n}),e.pendingConnects.length>1||e.initConnect();}))]}))}))},RealtimeService.prototype.initConnect=function(){var e=this;this.disconnect(!0),clearTimeout(this.connectTimeoutId),this.connectTimeoutId=setTimeout((function(){e.connectErrorHandler(new Error("EventSource connect took too long."));}),this.maxConnectTimeout),this.eventSource=new EventSource(this.client.buildUrl("/api/realtime")),this.eventSource.onerror=function(t){e.connectErrorHandler(new Error("Failed to establish realtime connection."));},this.eventSource.addEventListener("PB_CONNECT",(function(t){var n=t;e.clientId=null==n?void 0:n.lastEventId,e.submitSubscriptions().then((function(){return __awaiter(e,void 0,void 0,(function(){var e;return __generator(this,(function(t){switch(t.label){case 0:e=3,t.label=1;case 1:return this.hasUnsentSubscriptions()&&e>0?(e--,[4,this.submitSubscriptions()]):[3,3];case 2:return t.sent(),[3,1];case 3:return [2]}}))}))})).then((function(){for(var t=0,n=e.pendingConnects;t<n.length;t++){n[t].resolve();}e.pendingConnects=[],e.reconnectAttempts=0,clearTimeout(e.reconnectTimeoutId),clearTimeout(e.connectTimeoutId);})).catch((function(t){e.clientId="",e.connectErrorHandler(t);}));}));},RealtimeService.prototype.hasUnsentSubscriptions=function(){var e=this.getNonEmptySubscriptionTopics();if(e.length!=this.lastSentTopics.length)return !0;for(var t=0,n=e;t<n.length;t++){var i=n[t];if(!this.lastSentTopics.includes(i))return !0}return !1},RealtimeService.prototype.connectErrorHandler=function(e){var n=this;if(clearTimeout(this.connectTimeoutId),clearTimeout(this.reconnectTimeoutId),!this.clientId&&!this.reconnectAttempts||this.reconnectAttempts>this.maxReconnectAttempts){for(var i=0,o=this.pendingConnects;i<o.length;i++){o[i].reject(new t(e));}this.disconnect();}else {this.disconnect(!0);var r=this.predefinedReconnectIntervals[this.reconnectAttempts]||this.predefinedReconnectIntervals[this.predefinedReconnectIntervals.length-1];this.reconnectAttempts++,this.reconnectTimeoutId=setTimeout((function(){n.initConnect();}),r);}},RealtimeService.prototype.disconnect=function(e){var n;if(void 0===e&&(e=!1),clearTimeout(this.connectTimeoutId),clearTimeout(this.reconnectTimeoutId),this.removeAllSubscriptionListeners(),null===(n=this.eventSource)||void 0===n||n.close(),this.eventSource=null,this.clientId="",!e){this.reconnectAttempts=0;for(var i=new t(new Error("Realtime disconnected.")),o=0,r=this.pendingConnects;o<r.length;o++){r[o].reject(i);}this.pendingConnects=[];}},RealtimeService}(c),w=function(e){function HealthService(){return null!==e&&e.apply(this,arguments)||this}return __extends(HealthService,e),HealthService.prototype.check=function(e){return void 0===e&&(e={}),this.client.send("/api/health",{method:"GET",params:e})},HealthService}(c),C=function(){function Client(e,t,n){void 0===e&&(e="/"),void 0===n&&(n="en-US"),this.cancelControllers={},this.recordServices={},this.enableAutoCancellation=!0,this.baseUrl=e,this.lang=n,this.authStore=t||new a,this.admins=new h(this),this.collections=new b(this),this.logs=new g(this),this.settings=new u(this),this.realtime=new S(this),this.health=new w(this);}return Client.prototype.collection=function(e){return this.recordServices[e]||(this.recordServices[e]=new v(this,e)),this.recordServices[e]},Client.prototype.autoCancellation=function(e){return this.enableAutoCancellation=!!e,this},Client.prototype.cancelRequest=function(e){return this.cancelControllers[e]&&(this.cancelControllers[e].abort(),delete this.cancelControllers[e]),this},Client.prototype.cancelAllRequests=function(){for(var e in this.cancelControllers)this.cancelControllers[e].abort();return this.cancelControllers={},this},Client.prototype.send=function(e,n){var i,o,r,s,a,c,u,l;return __awaiter(this,void 0,void 0,(function(){var d,h,p,v,f,m=this;return __generator(this,(function(b){return (d=Object.assign({method:"GET"},n)).body&&"FormData"!==d.body.constructor.name&&("string"!=typeof d.body&&(d.body=JSON.stringify(d.body)),void 0===(null===(i=null==d?void 0:d.headers)||void 0===i?void 0:i["Content-Type"])&&(d.headers=Object.assign({},d.headers,{"Content-Type":"application/json"}))),void 0===(null===(o=null==d?void 0:d.headers)||void 0===o?void 0:o["Accept-Language"])&&(d.headers=Object.assign({},d.headers,{"Accept-Language":this.lang})),(null===(r=this.authStore)||void 0===r?void 0:r.token)&&void 0===(null===(s=null==d?void 0:d.headers)||void 0===s?void 0:s.Authorization)&&(d.headers=Object.assign({},d.headers,{Authorization:this.authStore.token})),this.enableAutoCancellation&&!1!==(null===(a=d.params)||void 0===a?void 0:a.$autoCancel)&&(h=(null===(c=d.params)||void 0===c?void 0:c.$cancelKey)||(d.method||"GET")+e,this.cancelRequest(h),p=new AbortController,this.cancelControllers[h]=p,d.signal=p.signal),null===(u=d.params)||void 0===u||delete u.$autoCancel,null===(l=d.params)||void 0===l||delete l.$cancelKey,v=this.buildUrl(e),void 0!==d.params&&((f=this.serializeQueryParams(d.params))&&(v+=(v.includes("?")?"&":"?")+f),delete d.params),this.beforeSend&&(d=Object.assign({},this.beforeSend(v,d))),[2,fetch(v,d).then((function(e){return __awaiter(m,void 0,void 0,(function(){var n;return __generator(this,(function(i){switch(i.label){case 0:n={},i.label=1;case 1:return i.trys.push([1,3,,4]),[4,e.json()];case 2:return n=i.sent(),[3,4];case 3:return i.sent(),[3,4];case 4:if(this.afterSend&&(n=this.afterSend(e,n)),e.status>=400)throw new t({url:e.url,status:e.status,data:n});return [2,n]}}))}))})).catch((function(e){throw new t(e)}))]}))}))},Client.prototype.getFileUrl=function(e,t,n){void 0===n&&(n={});var i=[];i.push("api"),i.push("files"),i.push(encodeURIComponent(e.collectionId||e.collectionName)),i.push(encodeURIComponent(e.id)),i.push(encodeURIComponent(t));var o=this.buildUrl(i.join("/"));if(Object.keys(n).length){var r=new URLSearchParams(n);o+=(o.includes("?")?"&":"?")+r;}return o},Client.prototype.buildUrl=function(e){var t=this.baseUrl+(this.baseUrl.endsWith("/")?"":"/");return e&&(t+=e.startsWith("/")?e.substring(1):e),t},Client.prototype.serializeQueryParams=function(e){var t=[];for(var n in e)if(null!==e[n]){var i=e[n],o=encodeURIComponent(n);if(Array.isArray(i))for(var r=0,s=i;r<s.length;r++){var a=s[r];t.push(o+"="+encodeURIComponent(a));}else i instanceof Date?t.push(o+"="+encodeURIComponent(i.toISOString())):null!==typeof i&&"object"==typeof i?t.push(o+"="+encodeURIComponent(JSON.stringify(i))):t.push(o+"="+encodeURIComponent(i));}return t.join("&")},Client}();

    const pb = new C('http://127.0.0.1:8090');
    const database = pb;
    function createUser(dto) {
        const promise = pb.collection('users').create(dto);
        promise.then(x => console.log(x)).catch(err => console.log(err));
    }

    var dbService = /*#__PURE__*/Object.freeze({
        __proto__: null,
        createUser: createUser,
        database: database
    });

    function mapEntityBase(r, x) {
        const entityBase = {
            id: r.id,
            created: new Date(r.created),
            updated: new Date(r.updated),
        };
        return Object.assign(x, entityBase);
    }
    function mapCategory(x) {
        const entity = {
            name: x.name,
            parentCategoryId: x.parentCategoryId,
        };
        return mapEntityBase(x, entity);
    }
    function mapElement(x) {
        const entity = {
            name: x.name,
            categoryId: x.categoryId,
            unitOfMeasureId: x.unitOfMeasureId
        };
        return mapEntityBase(x, entity);
    }
    function mapElementPurchasedEvent(x) {
        const entity = {
            elementId: x.elementId,
            numOfUnits: x.numOfUnits,
            unitPrice: x.unitPrice
        };
        return mapEntityBase(x, entity);
    }
    function mapExcerpt(x) {
        const entity = {
            compositeId: x.compositeId,
            elementId: x.elementId,
            quantity: x.quantity
        };
        return mapEntityBase(x, entity);
    }
    function mapUnitOfMeasure(x) {
        const entity = {
            name: x.name,
            symbol: x.symbol
        };
        return mapEntityBase(x, entity);
    }
    function mapExpand(r, expandKey, mapper) {
        const data = r.expand[expandKey];
        if (Array.isArray(data)) {
            return data.map(mapper);
        }
        else {
            return [mapper(data)];
        }
    }
    async function handlePromise(dbCall, message) {
        show(message);
        try {
            const data = await dbCall();
            hide();
            return data;
        }
        catch (error) {
            hide();
            console.log(error);
        }
    }

    const coll$1 = 'categories';
    async function getCategories() {
        const promiseFn = () => database.collection(coll$1).getFullList();
        const message = 'Fetching categories...';
        const data = await handlePromise(promiseFn, message);
        return data.map(mapCategory);
    }

    var categoryController = /*#__PURE__*/Object.freeze({
        __proto__: null,
        getCategories: getCategories
    });

    /* src\components\category\CategoryOverview.svelte generated by Svelte v3.55.1 */
    const file$5 = "src\\components\\category\\CategoryOverview.svelte";

    function get_each_context$1(ctx, list, i) {
    	const child_ctx = ctx.slice();
    	child_ctx[3] = list[i];
    	return child_ctx;
    }

    // (27:8) {#each mainCategories as ctg}
    function create_each_block$1(ctx) {
    	let categorydropdown;
    	let current;

    	categorydropdown = new CategoryDropdown({
    			props: {
    				category: /*ctg*/ ctx[3],
    				allCategories: /*categories*/ ctx[0]
    			},
    			$$inline: true
    		});

    	const block = {
    		c: function create() {
    			create_component(categorydropdown.$$.fragment);
    		},
    		m: function mount(target, anchor) {
    			mount_component(categorydropdown, target, anchor);
    			current = true;
    		},
    		p: function update(ctx, dirty) {
    			const categorydropdown_changes = {};
    			if (dirty & /*mainCategories*/ 2) categorydropdown_changes.category = /*ctg*/ ctx[3];
    			if (dirty & /*categories*/ 1) categorydropdown_changes.allCategories = /*categories*/ ctx[0];
    			categorydropdown.$set(categorydropdown_changes);
    		},
    		i: function intro(local) {
    			if (current) return;
    			transition_in(categorydropdown.$$.fragment, local);
    			current = true;
    		},
    		o: function outro(local) {
    			transition_out(categorydropdown.$$.fragment, local);
    			current = false;
    		},
    		d: function destroy(detaching) {
    			destroy_component(categorydropdown, detaching);
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_each_block$1.name,
    		type: "each",
    		source: "(27:8) {#each mainCategories as ctg}",
    		ctx
    	});

    	return block;
    }

    function create_fragment$6(ctx) {
    	let h2;
    	let t1;
    	let section;
    	let div0;
    	let h30;
    	let t3;
    	let t4;
    	let div1;
    	let h31;
    	let t6;
    	let button0;
    	let i0;
    	let t7;
    	let button1;
    	let i1;
    	let t8;
    	let button2;
    	let i2;
    	let t9;
    	let div4;
    	let h32;
    	let t11;
    	let div2;
    	let t13;
    	let div3;
    	let current;
    	let each_value = /*mainCategories*/ ctx[1];
    	validate_each_argument(each_value);
    	let each_blocks = [];

    	for (let i = 0; i < each_value.length; i += 1) {
    		each_blocks[i] = create_each_block$1(get_each_context$1(ctx, each_value, i));
    	}

    	const out = i => transition_out(each_blocks[i], 1, 1, () => {
    		each_blocks[i] = null;
    	});

    	const block = {
    		c: function create() {
    			h2 = element("h2");
    			h2.textContent = "Categories";
    			t1 = space();
    			section = element("section");
    			div0 = element("div");
    			h30 = element("h3");
    			h30.textContent = "Tree";
    			t3 = space();

    			for (let i = 0; i < each_blocks.length; i += 1) {
    				each_blocks[i].c();
    			}

    			t4 = space();
    			div1 = element("div");
    			h31 = element("h3");
    			h31.textContent = "Actions";
    			t6 = space();
    			button0 = element("button");
    			i0 = element("i");
    			t7 = space();
    			button1 = element("button");
    			i1 = element("i");
    			t8 = space();
    			button2 = element("button");
    			i2 = element("i");
    			t9 = space();
    			div4 = element("div");
    			h32 = element("h3");
    			h32.textContent = "Info";
    			t11 = space();
    			div2 = element("div");
    			div2.textContent = "Composites: 0";
    			t13 = space();
    			div3 = element("div");
    			div3.textContent = "Elements: 0";
    			attr_dev(h2, "class", "svelte-8i1ol0");
    			add_location(h2, file$5, 19, 0, 736);
    			attr_dev(h30, "class", "svelte-8i1ol0");
    			add_location(h30, file$5, 25, 8, 815);
    			attr_dev(div0, "class", "dropdowns svelte-8i1ol0");
    			add_location(div0, file$5, 24, 4, 782);
    			attr_dev(h31, "class", "svelte-8i1ol0");
    			add_location(h31, file$5, 31, 8, 1009);
    			attr_dev(i0, "class", "las la-plus");
    			add_location(i0, file$5, 33, 12, 1057);
    			attr_dev(button0, "class", "svelte-8i1ol0");
    			add_location(button0, file$5, 32, 8, 1035);
    			attr_dev(i1, "class", "las la-chart-line");
    			add_location(i1, file$5, 36, 12, 1135);
    			attr_dev(button1, "class", "svelte-8i1ol0");
    			add_location(button1, file$5, 35, 8, 1113);
    			attr_dev(i2, "class", "las la-trash");
    			add_location(i2, file$5, 39, 12, 1219);
    			attr_dev(button2, "class", "svelte-8i1ol0");
    			add_location(button2, file$5, 38, 8, 1197);
    			attr_dev(div1, "class", "actions svelte-8i1ol0");
    			add_location(div1, file$5, 30, 4, 978);
    			attr_dev(h32, "class", "svelte-8i1ol0");
    			add_location(h32, file$5, 43, 8, 1299);
    			add_location(div2, file$5, 44, 8, 1322);
    			add_location(div3, file$5, 47, 8, 1380);
    			attr_dev(div4, "class", "svelte-8i1ol0");
    			add_location(div4, file$5, 42, 4, 1284);
    			attr_dev(section, "class", "svelte-8i1ol0");
    			add_location(section, file$5, 23, 0, 767);
    		},
    		l: function claim(nodes) {
    			throw new Error("options.hydrate only works if the component was compiled with the `hydratable: true` option");
    		},
    		m: function mount(target, anchor) {
    			insert_dev(target, h2, anchor);
    			insert_dev(target, t1, anchor);
    			insert_dev(target, section, anchor);
    			append_dev(section, div0);
    			append_dev(div0, h30);
    			append_dev(div0, t3);

    			for (let i = 0; i < each_blocks.length; i += 1) {
    				each_blocks[i].m(div0, null);
    			}

    			append_dev(section, t4);
    			append_dev(section, div1);
    			append_dev(div1, h31);
    			append_dev(div1, t6);
    			append_dev(div1, button0);
    			append_dev(button0, i0);
    			append_dev(div1, t7);
    			append_dev(div1, button1);
    			append_dev(button1, i1);
    			append_dev(div1, t8);
    			append_dev(div1, button2);
    			append_dev(button2, i2);
    			append_dev(section, t9);
    			append_dev(section, div4);
    			append_dev(div4, h32);
    			append_dev(div4, t11);
    			append_dev(div4, div2);
    			append_dev(div4, t13);
    			append_dev(div4, div3);
    			current = true;
    		},
    		p: function update(ctx, [dirty]) {
    			if (dirty & /*mainCategories, categories*/ 3) {
    				each_value = /*mainCategories*/ ctx[1];
    				validate_each_argument(each_value);
    				let i;

    				for (i = 0; i < each_value.length; i += 1) {
    					const child_ctx = get_each_context$1(ctx, each_value, i);

    					if (each_blocks[i]) {
    						each_blocks[i].p(child_ctx, dirty);
    						transition_in(each_blocks[i], 1);
    					} else {
    						each_blocks[i] = create_each_block$1(child_ctx);
    						each_blocks[i].c();
    						transition_in(each_blocks[i], 1);
    						each_blocks[i].m(div0, null);
    					}
    				}

    				group_outros();

    				for (i = each_value.length; i < each_blocks.length; i += 1) {
    					out(i);
    				}

    				check_outros();
    			}
    		},
    		i: function intro(local) {
    			if (current) return;

    			for (let i = 0; i < each_value.length; i += 1) {
    				transition_in(each_blocks[i]);
    			}

    			current = true;
    		},
    		o: function outro(local) {
    			each_blocks = each_blocks.filter(Boolean);

    			for (let i = 0; i < each_blocks.length; i += 1) {
    				transition_out(each_blocks[i]);
    			}

    			current = false;
    		},
    		d: function destroy(detaching) {
    			if (detaching) detach_dev(h2);
    			if (detaching) detach_dev(t1);
    			if (detaching) detach_dev(section);
    			destroy_each(each_blocks, detaching);
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_fragment$6.name,
    		type: "component",
    		source: "",
    		ctx
    	});

    	return block;
    }

    function instance$6($$self, $$props, $$invalidate) {
    	let mainCategories;
    	let { $$slots: slots = {}, $$scope } = $$props;
    	validate_slots('CategoryOverview', slots, []);

    	onMount(() => {
    		getCategories().then(x => {
    			$$invalidate(0, categories = x);
    			selectedCategoryId$.set(x[0].id);
    		});

    		selectedCategoryId$.subscribe(x => {
    			selectedCategory = categories.find(c => c.id === x);
    		});
    	});

    	let categories = [];
    	let selectedCategory;
    	const writable_props = [];

    	Object.keys($$props).forEach(key => {
    		if (!~writable_props.indexOf(key) && key.slice(0, 2) !== '$$' && key !== 'slot') console.warn(`<CategoryOverview> was created with unknown prop '${key}'`);
    	});

    	$$self.$capture_state = () => ({
    		onMount,
    		CategoryDetails,
    		CategoryDropdown,
    		categoryService,
    		categoryController,
    		categories,
    		selectedCategory,
    		mainCategories
    	});

    	$$self.$inject_state = $$props => {
    		if ('categories' in $$props) $$invalidate(0, categories = $$props.categories);
    		if ('selectedCategory' in $$props) selectedCategory = $$props.selectedCategory;
    		if ('mainCategories' in $$props) $$invalidate(1, mainCategories = $$props.mainCategories);
    	};

    	if ($$props && "$$inject" in $$props) {
    		$$self.$inject_state($$props.$$inject);
    	}

    	$$self.$$.update = () => {
    		if ($$self.$$.dirty & /*categories*/ 1) {
    			$$invalidate(1, mainCategories = categories.filter(x => !x.parentCategoryId));
    		}
    	};

    	return [categories, mainCategories];
    }

    class CategoryOverview extends SvelteComponentDev {
    	constructor(options) {
    		super(options);
    		init(this, options, instance$6, create_fragment$6, safe_not_equal, {});

    		dispatch_dev("SvelteRegisterComponent", {
    			component: this,
    			tagName: "CategoryOverview",
    			options,
    			id: create_fragment$6.name
    		});
    	}
    }

    const coll = 'elements';
    async function getAllElements() {
        const promiseFn = () => database.collection(coll).getFullList();
        const message = 'Fetching elements...';
        const data = await handlePromise(promiseFn, message);
        return data.map(mapElement);
    }
    async function getElementsOverviews() {
        const category = 'categoryId';
        const excerpt = 'excerpts(elementId)';
        const uom = 'unitOfMeasureId';
        const getData = () => database.collection('elements').getFullList({
            expand: [uom, excerpt, category].join(',')
        });
        const data = await handlePromise(getData, 'Fetching elements...');
        return {
            elements: data.map(mapElement),
            categories: data.map(x => mapExpand(x, category, mapCategory)).flat(),
            unitsOfMeasure: data.map(x => mapExpand(x, uom, mapUnitOfMeasure)).flat(),
            excerpts: data.map(x => mapExpand(x, excerpt, mapExcerpt)).flat()
        };
    }
    async function getElementsForTable() {
        const excerpt = 'excerpts(elementId)';
        const purchase = 'elementPurchasedEvents(elementId)';
        const uom = 'unitOfMeasureId';
        const getElementData = () => database.collection('elements').getFullList({
            expand: [purchase, uom, excerpt].join(',')
        });
        const elementData = await handlePromise(getElementData, 'Fetching elements...');
        ({
            elements: elementData.map(mapElement),
            purchaseEvents: elementData.map(x => mapExpand(x, purchase, mapElementPurchasedEvent)).flat(),
            unitsOfMeasure: elementData.map(x => mapExpand(x, uom, mapUnitOfMeasure)).flat(),
            excerpts: elementData.map(x => mapExpand(x, excerpt, mapExcerpt)).flat()
        });
        console.log(elementData);
        return elementData;
    }

    var elementController = /*#__PURE__*/Object.freeze({
        __proto__: null,
        getAllElements: getAllElements,
        getElementsForTable: getElementsForTable,
        getElementsOverviews: getElementsOverviews
    });

    /* src\components\element\ElementList.svelte generated by Svelte v3.55.1 */

    const { console: console_1$2 } = globals;
    const file$4 = "src\\components\\element\\ElementList.svelte";

    function get_each_context(ctx, list, i) {
    	const child_ctx = ctx.slice();
    	child_ctx[2] = list[i];
    	return child_ctx;
    }

    // (31:12) {#each elements as elem}
    function create_each_block(ctx) {
    	let tr;
    	let td0;
    	let t0_value = /*elem*/ ctx[2].name + "";
    	let t0;
    	let t1;
    	let td1;
    	let t2_value = /*elem*/ ctx[2].category + "";
    	let t2;
    	let t3;
    	let td2;
    	let t4_value = /*elem*/ ctx[2].unitOfMeasure + "";
    	let t4;
    	let t5;
    	let td3;
    	let t6_value = /*elem*/ ctx[2].compositesCount + "";
    	let t6;
    	let t7;
    	let div;
    	let t9;
    	let mounted;
    	let dispose;

    	function click_handler() {
    		return /*click_handler*/ ctx[1](/*elem*/ ctx[2]);
    	}

    	const block = {
    		c: function create() {
    			tr = element("tr");
    			td0 = element("td");
    			t0 = text(t0_value);
    			t1 = space();
    			td1 = element("td");
    			t2 = text(t2_value);
    			t3 = space();
    			td2 = element("td");
    			t4 = text(t4_value);
    			t5 = space();
    			td3 = element("td");
    			t6 = text(t6_value);
    			t7 = space();
    			div = element("div");
    			div.textContent = "aaaaa";
    			t9 = space();
    			attr_dev(td0, "class", "svelte-ce3poo");
    			add_location(td0, file$4, 32, 20, 1090);
    			attr_dev(td1, "class", "svelte-ce3poo");
    			add_location(td1, file$4, 33, 20, 1132);
    			attr_dev(td2, "class", "svelte-ce3poo");
    			add_location(td2, file$4, 34, 20, 1178);
    			attr_dev(td3, "class", "svelte-ce3poo");
    			add_location(td3, file$4, 35, 20, 1229);
    			attr_dev(div, "class", "svelte-ce3poo");
    			add_location(div, file$4, 36, 20, 1282);
    			attr_dev(tr, "class", "svelte-ce3poo");
    			add_location(tr, file$4, 31, 16, 1026);
    		},
    		m: function mount(target, anchor) {
    			insert_dev(target, tr, anchor);
    			append_dev(tr, td0);
    			append_dev(td0, t0);
    			append_dev(tr, t1);
    			append_dev(tr, td1);
    			append_dev(td1, t2);
    			append_dev(tr, t3);
    			append_dev(tr, td2);
    			append_dev(td2, t4);
    			append_dev(tr, t5);
    			append_dev(tr, td3);
    			append_dev(td3, t6);
    			append_dev(tr, t7);
    			append_dev(tr, div);
    			append_dev(tr, t9);

    			if (!mounted) {
    				dispose = listen_dev(tr, "click", click_handler, false, false, false);
    				mounted = true;
    			}
    		},
    		p: function update(new_ctx, dirty) {
    			ctx = new_ctx;
    			if (dirty & /*elements*/ 1 && t0_value !== (t0_value = /*elem*/ ctx[2].name + "")) set_data_dev(t0, t0_value);
    			if (dirty & /*elements*/ 1 && t2_value !== (t2_value = /*elem*/ ctx[2].category + "")) set_data_dev(t2, t2_value);
    			if (dirty & /*elements*/ 1 && t4_value !== (t4_value = /*elem*/ ctx[2].unitOfMeasure + "")) set_data_dev(t4, t4_value);
    			if (dirty & /*elements*/ 1 && t6_value !== (t6_value = /*elem*/ ctx[2].compositesCount + "")) set_data_dev(t6, t6_value);
    		},
    		d: function destroy(detaching) {
    			if (detaching) detach_dev(tr);
    			mounted = false;
    			dispose();
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_each_block.name,
    		type: "each",
    		source: "(31:12) {#each elements as elem}",
    		ctx
    	});

    	return block;
    }

    function create_fragment$5(ctx) {
    	let h2;
    	let t1;
    	let section;
    	let table;
    	let thead;
    	let tr;
    	let th0;
    	let t3;
    	let th1;
    	let t5;
    	let th2;
    	let t7;
    	let th3;
    	let t9;
    	let tbody;
    	let each_value = /*elements*/ ctx[0];
    	validate_each_argument(each_value);
    	let each_blocks = [];

    	for (let i = 0; i < each_value.length; i += 1) {
    		each_blocks[i] = create_each_block(get_each_context(ctx, each_value, i));
    	}

    	const block = {
    		c: function create() {
    			h2 = element("h2");
    			h2.textContent = "Elements";
    			t1 = space();
    			section = element("section");
    			table = element("table");
    			thead = element("thead");
    			tr = element("tr");
    			th0 = element("th");
    			th0.textContent = "Name";
    			t3 = space();
    			th1 = element("th");
    			th1.textContent = "Category";
    			t5 = space();
    			th2 = element("th");
    			th2.textContent = "Unit of Measure";
    			t7 = space();
    			th3 = element("th");
    			th3.textContent = "In composites";
    			t9 = space();
    			tbody = element("tbody");

    			for (let i = 0; i < each_blocks.length; i += 1) {
    				each_blocks[i].c();
    			}

    			attr_dev(h2, "class", "svelte-ce3poo");
    			add_location(h2, file$4, 18, 0, 692);
    			attr_dev(th0, "class", "svelte-ce3poo");
    			add_location(th0, file$4, 23, 16, 786);
    			attr_dev(th1, "class", "svelte-ce3poo");
    			add_location(th1, file$4, 24, 16, 817);
    			attr_dev(th2, "class", "svelte-ce3poo");
    			add_location(th2, file$4, 25, 16, 852);
    			attr_dev(th3, "class", "svelte-ce3poo");
    			add_location(th3, file$4, 26, 16, 894);
    			attr_dev(tr, "class", "svelte-ce3poo");
    			add_location(tr, file$4, 22, 12, 764);
    			attr_dev(thead, "class", "svelte-ce3poo");
    			add_location(thead, file$4, 21, 8, 743);
    			attr_dev(tbody, "class", "svelte-ce3poo");
    			add_location(tbody, file$4, 29, 8, 963);
    			attr_dev(table, "class", "svelte-ce3poo");
    			add_location(table, file$4, 20, 4, 726);
    			attr_dev(section, "class", "svelte-ce3poo");
    			add_location(section, file$4, 19, 0, 711);
    		},
    		l: function claim(nodes) {
    			throw new Error("options.hydrate only works if the component was compiled with the `hydratable: true` option");
    		},
    		m: function mount(target, anchor) {
    			insert_dev(target, h2, anchor);
    			insert_dev(target, t1, anchor);
    			insert_dev(target, section, anchor);
    			append_dev(section, table);
    			append_dev(table, thead);
    			append_dev(thead, tr);
    			append_dev(tr, th0);
    			append_dev(tr, t3);
    			append_dev(tr, th1);
    			append_dev(tr, t5);
    			append_dev(tr, th2);
    			append_dev(tr, t7);
    			append_dev(tr, th3);
    			append_dev(table, t9);
    			append_dev(table, tbody);

    			for (let i = 0; i < each_blocks.length; i += 1) {
    				each_blocks[i].m(tbody, null);
    			}
    		},
    		p: function update(ctx, [dirty]) {
    			if (dirty & /*console, elements*/ 1) {
    				each_value = /*elements*/ ctx[0];
    				validate_each_argument(each_value);
    				let i;

    				for (i = 0; i < each_value.length; i += 1) {
    					const child_ctx = get_each_context(ctx, each_value, i);

    					if (each_blocks[i]) {
    						each_blocks[i].p(child_ctx, dirty);
    					} else {
    						each_blocks[i] = create_each_block(child_ctx);
    						each_blocks[i].c();
    						each_blocks[i].m(tbody, null);
    					}
    				}

    				for (; i < each_blocks.length; i += 1) {
    					each_blocks[i].d(1);
    				}

    				each_blocks.length = each_value.length;
    			}
    		},
    		i: noop,
    		o: noop,
    		d: function destroy(detaching) {
    			if (detaching) detach_dev(h2);
    			if (detaching) detach_dev(t1);
    			if (detaching) detach_dev(section);
    			destroy_each(each_blocks, detaching);
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_fragment$5.name,
    		type: "component",
    		source: "",
    		ctx
    	});

    	return block;
    }

    function instance$5($$self, $$props, $$invalidate) {
    	let { $$slots: slots = {}, $$scope } = $$props;
    	validate_slots('ElementList', slots, []);

    	onMount(() => {
    		getElementsOverviews().then(data => {
    			$$invalidate(0, elements = data.elements.map(el => {
    				return {
    					id: el.id,
    					name: el.name,
    					category: data.categories.find(x => x.id === el.categoryId).name,
    					unitOfMeasure: data.unitsOfMeasure.find(x => x.id === el.unitOfMeasureId).name,
    					compositesCount: data.excerpts.filter(x => x.elementId === el.id).length
    				};
    			}));
    		});
    	});

    	let elements = [];
    	const writable_props = [];

    	Object.keys($$props).forEach(key => {
    		if (!~writable_props.indexOf(key) && key.slice(0, 2) !== '$$' && key !== 'slot') console_1$2.warn(`<ElementList> was created with unknown prop '${key}'`);
    	});

    	const click_handler = elem => console.log(elem.id);
    	$$self.$capture_state = () => ({ onMount, elementController, elements });

    	$$self.$inject_state = $$props => {
    		if ('elements' in $$props) $$invalidate(0, elements = $$props.elements);
    	};

    	if ($$props && "$$inject" in $$props) {
    		$$self.$inject_state($$props.$$inject);
    	}

    	return [elements, click_handler];
    }

    class ElementList extends SvelteComponentDev {
    	constructor(options) {
    		super(options);
    		init(this, options, instance$5, create_fragment$5, safe_not_equal, {});

    		dispatch_dev("SvelteRegisterComponent", {
    			component: this,
    			tagName: "ElementList",
    			options,
    			id: create_fragment$5.name
    		});
    	}
    }

    /* src\components\home\Home.svelte generated by Svelte v3.55.1 */

    const { console: console_1$1 } = globals;
    const file$3 = "src\\components\\home\\Home.svelte";

    // (22:8) {#if selectedComponent}
    function create_if_block$1(ctx) {
    	let switch_instance;
    	let switch_instance_anchor;
    	let current;
    	var switch_value = /*selectedComponent*/ ctx[0];

    	function switch_props(ctx) {
    		return { $$inline: true };
    	}

    	if (switch_value) {
    		switch_instance = construct_svelte_component_dev(switch_value, switch_props());
    	}

    	const block = {
    		c: function create() {
    			if (switch_instance) create_component(switch_instance.$$.fragment);
    			switch_instance_anchor = empty();
    		},
    		m: function mount(target, anchor) {
    			if (switch_instance) mount_component(switch_instance, target, anchor);
    			insert_dev(target, switch_instance_anchor, anchor);
    			current = true;
    		},
    		p: function update(ctx, dirty) {
    			if (switch_value !== (switch_value = /*selectedComponent*/ ctx[0])) {
    				if (switch_instance) {
    					group_outros();
    					const old_component = switch_instance;

    					transition_out(old_component.$$.fragment, 1, 0, () => {
    						destroy_component(old_component, 1);
    					});

    					check_outros();
    				}

    				if (switch_value) {
    					switch_instance = construct_svelte_component_dev(switch_value, switch_props());
    					create_component(switch_instance.$$.fragment);
    					transition_in(switch_instance.$$.fragment, 1);
    					mount_component(switch_instance, switch_instance_anchor.parentNode, switch_instance_anchor);
    				} else {
    					switch_instance = null;
    				}
    			}
    		},
    		i: function intro(local) {
    			if (current) return;
    			if (switch_instance) transition_in(switch_instance.$$.fragment, local);
    			current = true;
    		},
    		o: function outro(local) {
    			if (switch_instance) transition_out(switch_instance.$$.fragment, local);
    			current = false;
    		},
    		d: function destroy(detaching) {
    			if (detaching) detach_dev(switch_instance_anchor);
    			if (switch_instance) destroy_component(switch_instance, detaching);
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_if_block$1.name,
    		type: "if",
    		source: "(22:8) {#if selectedComponent}",
    		ctx
    	});

    	return block;
    }

    function create_fragment$4(ctx) {
    	let section;
    	let homenav;
    	let t;
    	let div;
    	let current;
    	homenav = new HomeNav({ $$inline: true });
    	homenav.$on("componentSelected", /*onComponentSelected*/ ctx[1]);
    	let if_block = /*selectedComponent*/ ctx[0] && create_if_block$1(ctx);

    	const block = {
    		c: function create() {
    			section = element("section");
    			create_component(homenav.$$.fragment);
    			t = space();
    			div = element("div");
    			if (if_block) if_block.c();
    			attr_dev(div, "class", "svelte-1lnzti2");
    			add_location(div, file$3, 20, 4, 898);
    			attr_dev(section, "class", "svelte-1lnzti2");
    			add_location(section, file$3, 18, 0, 823);
    		},
    		l: function claim(nodes) {
    			throw new Error("options.hydrate only works if the component was compiled with the `hydratable: true` option");
    		},
    		m: function mount(target, anchor) {
    			insert_dev(target, section, anchor);
    			mount_component(homenav, section, null);
    			append_dev(section, t);
    			append_dev(section, div);
    			if (if_block) if_block.m(div, null);
    			current = true;
    		},
    		p: function update(ctx, [dirty]) {
    			if (/*selectedComponent*/ ctx[0]) {
    				if (if_block) {
    					if_block.p(ctx, dirty);

    					if (dirty & /*selectedComponent*/ 1) {
    						transition_in(if_block, 1);
    					}
    				} else {
    					if_block = create_if_block$1(ctx);
    					if_block.c();
    					transition_in(if_block, 1);
    					if_block.m(div, null);
    				}
    			} else if (if_block) {
    				group_outros();

    				transition_out(if_block, 1, 1, () => {
    					if_block = null;
    				});

    				check_outros();
    			}
    		},
    		i: function intro(local) {
    			if (current) return;
    			transition_in(homenav.$$.fragment, local);
    			transition_in(if_block);
    			current = true;
    		},
    		o: function outro(local) {
    			transition_out(homenav.$$.fragment, local);
    			transition_out(if_block);
    			current = false;
    		},
    		d: function destroy(detaching) {
    			if (detaching) detach_dev(section);
    			destroy_component(homenav);
    			if (if_block) if_block.d();
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_fragment$4.name,
    		type: "component",
    		source: "",
    		ctx
    	});

    	return block;
    }

    function instance$4($$self, $$props, $$invalidate) {
    	let { $$slots: slots = {}, $$scope } = $$props;
    	validate_slots('Home', slots, []);

    	const components = [
    		{
    			name: 'Categories',
    			component: CategoryOverview
    		},
    		{
    			name: 'Elements',
    			icon: 'las la-vial',
    			component: ElementList
    		},
    		{
    			name: 'Composites',
    			icon: 'las la-cubes',
    			component: null
    		},
    		{
    			name: 'Stocks',
    			icon: 'las la-warehouse',
    			component: null
    		},
    		{
    			name: 'Settings',
    			icon: 'las la-wrench',
    			component: null
    		},
    		{
    			name: 'Users',
    			icon: 'las la-users',
    			component: null
    		}
    	];

    	let selectedComponent = components[0].component;

    	function onComponentSelected(ev) {
    		console.log(ev.detail);
    		$$invalidate(0, selectedComponent = components.find(x => ev.detail.name === x.name).component);
    	}

    	const writable_props = [];

    	Object.keys($$props).forEach(key => {
    		if (!~writable_props.indexOf(key) && key.slice(0, 2) !== '$$' && key !== 'slot') console_1$1.warn(`<Home> was created with unknown prop '${key}'`);
    	});

    	$$self.$capture_state = () => ({
    		HomeNav,
    		CategoryOverview,
    		ElementList,
    		components,
    		selectedComponent,
    		onComponentSelected
    	});

    	$$self.$inject_state = $$props => {
    		if ('selectedComponent' in $$props) $$invalidate(0, selectedComponent = $$props.selectedComponent);
    	};

    	if ($$props && "$$inject" in $$props) {
    		$$self.$inject_state($$props.$$inject);
    	}

    	return [selectedComponent, onComponentSelected];
    }

    class Home extends SvelteComponentDev {
    	constructor(options) {
    		super(options);
    		init(this, options, instance$4, create_fragment$4, safe_not_equal, {});

    		dispatch_dev("SvelteRegisterComponent", {
    			component: this,
    			tagName: "Home",
    			options,
    			id: create_fragment$4.name
    		});
    	}
    }

    function checkAuthStore(onValidCredentials) {
        if (database.authStore.isValid) {
            onValidCredentials();
            return;
        }
    }
    function login(dto, callback) {
        show('Logging in');
        if (dto.asAdmin) {
            database.admins.authWithPassword(dto.email, dto.password)
                .then(x => {
                console.log(x);
                hide();
                callback();
            })
                .catch(e => {
                hide();
                console.log(e);
            });
        }
        else {
            console.log('regular user login not implemented');
        }
    }

    var userController = /*#__PURE__*/Object.freeze({
        __proto__: null,
        checkAuthStore: checkAuthStore,
        login: login
    });

    /* src\components\account\Login.svelte generated by Svelte v3.55.1 */

    const { console: console_1 } = globals;
    const file$2 = "src\\components\\account\\Login.svelte";

    function create_fragment$3(ctx) {
    	let section;
    	let fieldset;
    	let legend;
    	let t1;
    	let form;
    	let label0;
    	let input0;
    	let t2;
    	let label1;
    	let input1;
    	let t3;
    	let label2;
    	let t4;
    	let input2;
    	let t5;
    	let button;
    	let t7;
    	let div;
    	let i;
    	let t8;
    	let a;
    	let t10;
    	let mounted;
    	let dispose;

    	const block = {
    		c: function create() {
    			section = element("section");
    			fieldset = element("fieldset");
    			legend = element("legend");
    			legend.textContent = "Login";
    			t1 = space();
    			form = element("form");
    			label0 = element("label");
    			input0 = element("input");
    			t2 = space();
    			label1 = element("label");
    			input1 = element("input");
    			t3 = space();
    			label2 = element("label");
    			t4 = text("As admin ");
    			input2 = element("input");
    			t5 = space();
    			button = element("button");
    			button.textContent = "Submit";
    			t7 = space();
    			div = element("div");
    			i = element("i");
    			t8 = text("Or ");
    			a = element("a");
    			a.textContent = "register";
    			t10 = text(" if you don't have an account");
    			attr_dev(legend, "class", "svelte-7eiies");
    			add_location(legend, file$2, 24, 8, 642);
    			attr_dev(input0, "placeholder", "email");
    			attr_dev(input0, "type", "text");
    			add_location(input0, file$2, 27, 16, 759);
    			add_location(label0, file$2, 26, 12, 734);
    			attr_dev(input1, "placeholder", "password");
    			attr_dev(input1, "type", "password");
    			add_location(input1, file$2, 30, 16, 884);
    			add_location(label1, file$2, 29, 12, 859);
    			attr_dev(input2, "type", "checkbox");
    			add_location(input2, file$2, 33, 25, 1028);
    			add_location(label2, file$2, 32, 12, 994);
    			attr_dev(button, "type", "submit");
    			attr_dev(button, "class", "svelte-7eiies");
    			add_location(button, file$2, 35, 12, 1116);
    			attr_dev(a, "href", "#a");
    			add_location(a, file$2, 37, 22, 1213);
    			add_location(i, file$2, 37, 16, 1207);
    			attr_dev(div, "class", "register svelte-7eiies");
    			add_location(div, file$2, 36, 12, 1167);
    			add_location(form, file$2, 25, 8, 674);
    			add_location(fieldset, file$2, 23, 4, 622);
    			attr_dev(section, "class", "flex svelte-7eiies");
    			add_location(section, file$2, 22, 0, 594);
    		},
    		l: function claim(nodes) {
    			throw new Error("options.hydrate only works if the component was compiled with the `hydratable: true` option");
    		},
    		m: function mount(target, anchor) {
    			insert_dev(target, section, anchor);
    			append_dev(section, fieldset);
    			append_dev(fieldset, legend);
    			append_dev(fieldset, t1);
    			append_dev(fieldset, form);
    			append_dev(form, label0);
    			append_dev(label0, input0);
    			set_input_value(input0, /*dto*/ ctx[0].email);
    			append_dev(form, t2);
    			append_dev(form, label1);
    			append_dev(label1, input1);
    			set_input_value(input1, /*dto*/ ctx[0].password);
    			append_dev(form, t3);
    			append_dev(form, label2);
    			append_dev(label2, t4);
    			append_dev(label2, input2);
    			input2.checked = /*dto*/ ctx[0].asAdmin;
    			append_dev(form, t5);
    			append_dev(form, button);
    			append_dev(form, t7);
    			append_dev(form, div);
    			append_dev(div, i);
    			append_dev(i, t8);
    			append_dev(i, a);
    			append_dev(i, t10);

    			if (!mounted) {
    				dispose = [
    					listen_dev(input0, "input", /*input0_input_handler*/ ctx[3]),
    					listen_dev(input1, "input", /*input1_input_handler*/ ctx[4]),
    					listen_dev(input2, "change", /*input2_change_handler*/ ctx[5]),
    					listen_dev(a, "click", /*goToRegisterPage*/ ctx[2], false, false, false),
    					listen_dev(form, "submit", prevent_default(/*handleSubmit*/ ctx[1]), false, true, false)
    				];

    				mounted = true;
    			}
    		},
    		p: function update(ctx, [dirty]) {
    			if (dirty & /*dto*/ 1 && input0.value !== /*dto*/ ctx[0].email) {
    				set_input_value(input0, /*dto*/ ctx[0].email);
    			}

    			if (dirty & /*dto*/ 1 && input1.value !== /*dto*/ ctx[0].password) {
    				set_input_value(input1, /*dto*/ ctx[0].password);
    			}

    			if (dirty & /*dto*/ 1) {
    				input2.checked = /*dto*/ ctx[0].asAdmin;
    			}
    		},
    		i: noop,
    		o: noop,
    		d: function destroy(detaching) {
    			if (detaching) detach_dev(section);
    			mounted = false;
    			run_all(dispose);
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_fragment$3.name,
    		type: "component",
    		source: "",
    		ctx
    	});

    	return block;
    }

    function instance$3($$self, $$props, $$invalidate) {
    	let { $$slots: slots = {}, $$scope } = $$props;
    	validate_slots('Login', slots, []);

    	onMount(() => {
    		checkAuthStore(() => {
    			goto.home();
    		});
    	});

    	let dto = {
    		email: 'admin@admin.com',
    		password: 'adminadmin',
    		asAdmin: true
    	};

    	function handleSubmit() {
    		console.log(dto);
    		login(dto, () => goto.home());
    	}

    	function goToRegisterPage() {
    		goto.register();
    	}

    	const writable_props = [];

    	Object.keys($$props).forEach(key => {
    		if (!~writable_props.indexOf(key) && key.slice(0, 2) !== '$$' && key !== 'slot') console_1.warn(`<Login> was created with unknown prop '${key}'`);
    	});

    	function input0_input_handler() {
    		dto.email = this.value;
    		$$invalidate(0, dto);
    	}

    	function input1_input_handler() {
    		dto.password = this.value;
    		$$invalidate(0, dto);
    	}

    	function input2_change_handler() {
    		dto.asAdmin = this.checked;
    		$$invalidate(0, dto);
    	}

    	$$self.$capture_state = () => ({
    		onMount,
    		pageService,
    		userController,
    		dto,
    		handleSubmit,
    		goToRegisterPage
    	});

    	$$self.$inject_state = $$props => {
    		if ('dto' in $$props) $$invalidate(0, dto = $$props.dto);
    	};

    	if ($$props && "$$inject" in $$props) {
    		$$self.$inject_state($$props.$$inject);
    	}

    	return [
    		dto,
    		handleSubmit,
    		goToRegisterPage,
    		input0_input_handler,
    		input1_input_handler,
    		input2_change_handler
    	];
    }

    class Login extends SvelteComponentDev {
    	constructor(options) {
    		super(options);
    		init(this, options, instance$3, create_fragment$3, safe_not_equal, {});

    		dispatch_dev("SvelteRegisterComponent", {
    			component: this,
    			tagName: "Login",
    			options,
    			id: create_fragment$3.name
    		});
    	}
    }

    /* src\components\account\Register.svelte generated by Svelte v3.55.1 */
    const file$1 = "src\\components\\account\\Register.svelte";

    function create_fragment$2(ctx) {
    	let section;
    	let fieldset;
    	let legend;
    	let t1;
    	let form;
    	let label0;
    	let input0;
    	let t2;
    	let label1;
    	let input1;
    	let t3;
    	let label2;
    	let input2;
    	let t4;
    	let label3;
    	let input3;
    	let t5;
    	let div;
    	let button;
    	let mounted;
    	let dispose;

    	const block = {
    		c: function create() {
    			section = element("section");
    			fieldset = element("fieldset");
    			legend = element("legend");
    			legend.textContent = "Login";
    			t1 = space();
    			form = element("form");
    			label0 = element("label");
    			input0 = element("input");
    			t2 = space();
    			label1 = element("label");
    			input1 = element("input");
    			t3 = space();
    			label2 = element("label");
    			input2 = element("input");
    			t4 = space();
    			label3 = element("label");
    			input3 = element("input");
    			t5 = space();
    			div = element("div");
    			button = element("button");
    			button.textContent = "Submit";
    			add_location(legend, file$1, 14, 8, 302);
    			attr_dev(input0, "placeholder", "username");
    			attr_dev(input0, "type", "text");
    			add_location(input0, file$1, 17, 16, 419);
    			add_location(label0, file$1, 16, 12, 394);
    			attr_dev(input1, "placeholder", "email");
    			attr_dev(input1, "type", "text");
    			add_location(input1, file$1, 20, 16, 548);
    			add_location(label1, file$1, 19, 12, 523);
    			attr_dev(input2, "placeholder", "password");
    			attr_dev(input2, "type", "password");
    			add_location(input2, file$1, 23, 16, 671);
    			add_location(label2, file$1, 22, 12, 646);
    			attr_dev(input3, "placeholder", "confirm password");
    			attr_dev(input3, "type", "password");
    			add_location(input3, file$1, 26, 16, 804);
    			add_location(label3, file$1, 25, 12, 779);
    			attr_dev(button, "type", "submit");
    			add_location(button, file$1, 29, 16, 963);
    			attr_dev(div, "class", "flex");
    			add_location(div, file$1, 28, 12, 927);
    			add_location(form, file$1, 15, 8, 334);
    			add_location(fieldset, file$1, 13, 4, 282);
    			attr_dev(section, "class", "flex");
    			add_location(section, file$1, 12, 0, 254);
    		},
    		l: function claim(nodes) {
    			throw new Error("options.hydrate only works if the component was compiled with the `hydratable: true` option");
    		},
    		m: function mount(target, anchor) {
    			insert_dev(target, section, anchor);
    			append_dev(section, fieldset);
    			append_dev(fieldset, legend);
    			append_dev(fieldset, t1);
    			append_dev(fieldset, form);
    			append_dev(form, label0);
    			append_dev(label0, input0);
    			set_input_value(input0, /*dto*/ ctx[0].username);
    			append_dev(form, t2);
    			append_dev(form, label1);
    			append_dev(label1, input1);
    			set_input_value(input1, /*dto*/ ctx[0].email);
    			append_dev(form, t3);
    			append_dev(form, label2);
    			append_dev(label2, input2);
    			set_input_value(input2, /*dto*/ ctx[0].password);
    			append_dev(form, t4);
    			append_dev(form, label3);
    			append_dev(label3, input3);
    			set_input_value(input3, /*dto*/ ctx[0].passwordConfirm);
    			append_dev(form, t5);
    			append_dev(form, div);
    			append_dev(div, button);

    			if (!mounted) {
    				dispose = [
    					listen_dev(input0, "input", /*input0_input_handler*/ ctx[2]),
    					listen_dev(input1, "input", /*input1_input_handler*/ ctx[3]),
    					listen_dev(input2, "input", /*input2_input_handler*/ ctx[4]),
    					listen_dev(input3, "input", /*input3_input_handler*/ ctx[5]),
    					listen_dev(form, "submit", prevent_default(/*handleSubmit*/ ctx[1]), false, true, false)
    				];

    				mounted = true;
    			}
    		},
    		p: function update(ctx, [dirty]) {
    			if (dirty & /*dto*/ 1 && input0.value !== /*dto*/ ctx[0].username) {
    				set_input_value(input0, /*dto*/ ctx[0].username);
    			}

    			if (dirty & /*dto*/ 1 && input1.value !== /*dto*/ ctx[0].email) {
    				set_input_value(input1, /*dto*/ ctx[0].email);
    			}

    			if (dirty & /*dto*/ 1 && input2.value !== /*dto*/ ctx[0].password) {
    				set_input_value(input2, /*dto*/ ctx[0].password);
    			}

    			if (dirty & /*dto*/ 1 && input3.value !== /*dto*/ ctx[0].passwordConfirm) {
    				set_input_value(input3, /*dto*/ ctx[0].passwordConfirm);
    			}
    		},
    		i: noop,
    		o: noop,
    		d: function destroy(detaching) {
    			if (detaching) detach_dev(section);
    			mounted = false;
    			run_all(dispose);
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_fragment$2.name,
    		type: "component",
    		source: "",
    		ctx
    	});

    	return block;
    }

    function instance$2($$self, $$props, $$invalidate) {
    	let { $$slots: slots = {}, $$scope } = $$props;
    	validate_slots('Register', slots, []);

    	let dto = {
    		username: '',
    		email: '',
    		password: '',
    		passwordConfirm: ''
    	};

    	function handleSubmit() {
    		createUser(dto);
    	}

    	const writable_props = [];

    	Object.keys($$props).forEach(key => {
    		if (!~writable_props.indexOf(key) && key.slice(0, 2) !== '$$' && key !== 'slot') console.warn(`<Register> was created with unknown prop '${key}'`);
    	});

    	function input0_input_handler() {
    		dto.username = this.value;
    		$$invalidate(0, dto);
    	}

    	function input1_input_handler() {
    		dto.email = this.value;
    		$$invalidate(0, dto);
    	}

    	function input2_input_handler() {
    		dto.password = this.value;
    		$$invalidate(0, dto);
    	}

    	function input3_input_handler() {
    		dto.passwordConfirm = this.value;
    		$$invalidate(0, dto);
    	}

    	$$self.$capture_state = () => ({ dbService, dto, handleSubmit });

    	$$self.$inject_state = $$props => {
    		if ('dto' in $$props) $$invalidate(0, dto = $$props.dto);
    	};

    	if ($$props && "$$inject" in $$props) {
    		$$self.$inject_state($$props.$$inject);
    	}

    	return [
    		dto,
    		handleSubmit,
    		input0_input_handler,
    		input1_input_handler,
    		input2_input_handler,
    		input3_input_handler
    	];
    }

    class Register extends SvelteComponentDev {
    	constructor(options) {
    		super(options);
    		init(this, options, instance$2, create_fragment$2, safe_not_equal, {});

    		dispatch_dev("SvelteRegisterComponent", {
    			component: this,
    			tagName: "Register",
    			options,
    			id: create_fragment$2.name
    		});
    	}
    }

    const pages = [
        { name: '', component: Login, props: null },
        { name: 'login', component: Login, props: null },
        { name: 'register', component: Register, props: null },
        { name: 'home', component: Home, props: null },
        { name: 'test', component: Home, props: null }
    ];
    const currentPage$ = writable(pages[0]);
    function changePage(name, props) {
        const page = pages.find(x => x.name === name);
        page.props = props;
        currentPage$.set(page);
    }
    const onCurrentPageChanged = currentPage$.subscribe;
    const goto = {
        default: function () {
            changePage('');
        },
        login: function () {
            changePage('login');
        },
        register: function () {
            changePage('register');
        },
        home: function () {
            changePage('home');
        },
        test: function (props) {
            changePage('home', props);
        }
    };

    /* src\components\commonUI\PageSelector.svelte generated by Svelte v3.55.1 */

    // (9:0) {#if selected}
    function create_if_block(ctx) {
    	let switch_instance;
    	let switch_instance_anchor;
    	let current;
    	const switch_instance_spread_levels = [/*selected*/ ctx[0].props];
    	var switch_value = /*selected*/ ctx[0].component;

    	function switch_props(ctx) {
    		let switch_instance_props = {};

    		for (let i = 0; i < switch_instance_spread_levels.length; i += 1) {
    			switch_instance_props = assign(switch_instance_props, switch_instance_spread_levels[i]);
    		}

    		return {
    			props: switch_instance_props,
    			$$inline: true
    		};
    	}

    	if (switch_value) {
    		switch_instance = construct_svelte_component_dev(switch_value, switch_props());
    	}

    	const block = {
    		c: function create() {
    			if (switch_instance) create_component(switch_instance.$$.fragment);
    			switch_instance_anchor = empty();
    		},
    		m: function mount(target, anchor) {
    			if (switch_instance) mount_component(switch_instance, target, anchor);
    			insert_dev(target, switch_instance_anchor, anchor);
    			current = true;
    		},
    		p: function update(ctx, dirty) {
    			const switch_instance_changes = (dirty & /*selected*/ 1)
    			? get_spread_update(switch_instance_spread_levels, [get_spread_object(/*selected*/ ctx[0].props)])
    			: {};

    			if (switch_value !== (switch_value = /*selected*/ ctx[0].component)) {
    				if (switch_instance) {
    					group_outros();
    					const old_component = switch_instance;

    					transition_out(old_component.$$.fragment, 1, 0, () => {
    						destroy_component(old_component, 1);
    					});

    					check_outros();
    				}

    				if (switch_value) {
    					switch_instance = construct_svelte_component_dev(switch_value, switch_props());
    					create_component(switch_instance.$$.fragment);
    					transition_in(switch_instance.$$.fragment, 1);
    					mount_component(switch_instance, switch_instance_anchor.parentNode, switch_instance_anchor);
    				} else {
    					switch_instance = null;
    				}
    			} else if (switch_value) {
    				switch_instance.$set(switch_instance_changes);
    			}
    		},
    		i: function intro(local) {
    			if (current) return;
    			if (switch_instance) transition_in(switch_instance.$$.fragment, local);
    			current = true;
    		},
    		o: function outro(local) {
    			if (switch_instance) transition_out(switch_instance.$$.fragment, local);
    			current = false;
    		},
    		d: function destroy(detaching) {
    			if (detaching) detach_dev(switch_instance_anchor);
    			if (switch_instance) destroy_component(switch_instance, detaching);
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_if_block.name,
    		type: "if",
    		source: "(9:0) {#if selected}",
    		ctx
    	});

    	return block;
    }

    function create_fragment$1(ctx) {
    	let if_block_anchor;
    	let current;
    	let if_block = /*selected*/ ctx[0] && create_if_block(ctx);

    	const block = {
    		c: function create() {
    			if (if_block) if_block.c();
    			if_block_anchor = empty();
    		},
    		l: function claim(nodes) {
    			throw new Error("options.hydrate only works if the component was compiled with the `hydratable: true` option");
    		},
    		m: function mount(target, anchor) {
    			if (if_block) if_block.m(target, anchor);
    			insert_dev(target, if_block_anchor, anchor);
    			current = true;
    		},
    		p: function update(ctx, [dirty]) {
    			if (/*selected*/ ctx[0]) {
    				if (if_block) {
    					if_block.p(ctx, dirty);

    					if (dirty & /*selected*/ 1) {
    						transition_in(if_block, 1);
    					}
    				} else {
    					if_block = create_if_block(ctx);
    					if_block.c();
    					transition_in(if_block, 1);
    					if_block.m(if_block_anchor.parentNode, if_block_anchor);
    				}
    			} else if (if_block) {
    				group_outros();

    				transition_out(if_block, 1, 1, () => {
    					if_block = null;
    				});

    				check_outros();
    			}
    		},
    		i: function intro(local) {
    			if (current) return;
    			transition_in(if_block);
    			current = true;
    		},
    		o: function outro(local) {
    			transition_out(if_block);
    			current = false;
    		},
    		d: function destroy(detaching) {
    			if (if_block) if_block.d(detaching);
    			if (detaching) detach_dev(if_block_anchor);
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_fragment$1.name,
    		type: "component",
    		source: "",
    		ctx
    	});

    	return block;
    }

    function instance$1($$self, $$props, $$invalidate) {
    	let { $$slots: slots = {}, $$scope } = $$props;
    	validate_slots('PageSelector', slots, []);

    	onMount(() => {
    		onCurrentPageChanged(x => $$invalidate(0, selected = x));
    	});

    	let selected;
    	const writable_props = [];

    	Object.keys($$props).forEach(key => {
    		if (!~writable_props.indexOf(key) && key.slice(0, 2) !== '$$' && key !== 'slot') console.warn(`<PageSelector> was created with unknown prop '${key}'`);
    	});

    	$$self.$capture_state = () => ({ onMount, pageService, selected });

    	$$self.$inject_state = $$props => {
    		if ('selected' in $$props) $$invalidate(0, selected = $$props.selected);
    	};

    	if ($$props && "$$inject" in $$props) {
    		$$self.$inject_state($$props.$$inject);
    	}

    	return [selected];
    }

    class PageSelector extends SvelteComponentDev {
    	constructor(options) {
    		super(options);
    		init(this, options, instance$1, create_fragment$1, safe_not_equal, {});

    		dispatch_dev("SvelteRegisterComponent", {
    			component: this,
    			tagName: "PageSelector",
    			options,
    			id: create_fragment$1.name
    		});
    	}
    }

    /* src\App.svelte generated by Svelte v3.55.1 */
    const file = "src\\App.svelte";

    function create_fragment(ctx) {
    	let pageloader;
    	let t0;
    	let appheader;
    	let t1;
    	let main;
    	let pageselector;
    	let current;
    	pageloader = new PageLoader({ $$inline: true });
    	appheader = new AppHeader({ $$inline: true });
    	pageselector = new PageSelector({ $$inline: true });

    	const block = {
    		c: function create() {
    			create_component(pageloader.$$.fragment);
    			t0 = space();
    			create_component(appheader.$$.fragment);
    			t1 = space();
    			main = element("main");
    			create_component(pageselector.$$.fragment);
    			add_location(main, file, 8, 0, 302);
    		},
    		l: function claim(nodes) {
    			throw new Error("options.hydrate only works if the component was compiled with the `hydratable: true` option");
    		},
    		m: function mount(target, anchor) {
    			mount_component(pageloader, target, anchor);
    			insert_dev(target, t0, anchor);
    			mount_component(appheader, target, anchor);
    			insert_dev(target, t1, anchor);
    			insert_dev(target, main, anchor);
    			mount_component(pageselector, main, null);
    			current = true;
    		},
    		p: noop,
    		i: function intro(local) {
    			if (current) return;
    			transition_in(pageloader.$$.fragment, local);
    			transition_in(appheader.$$.fragment, local);
    			transition_in(pageselector.$$.fragment, local);
    			current = true;
    		},
    		o: function outro(local) {
    			transition_out(pageloader.$$.fragment, local);
    			transition_out(appheader.$$.fragment, local);
    			transition_out(pageselector.$$.fragment, local);
    			current = false;
    		},
    		d: function destroy(detaching) {
    			destroy_component(pageloader, detaching);
    			if (detaching) detach_dev(t0);
    			destroy_component(appheader, detaching);
    			if (detaching) detach_dev(t1);
    			if (detaching) detach_dev(main);
    			destroy_component(pageselector);
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_fragment.name,
    		type: "component",
    		source: "",
    		ctx
    	});

    	return block;
    }

    function instance($$self, $$props, $$invalidate) {
    	let { $$slots: slots = {}, $$scope } = $$props;
    	validate_slots('App', slots, []);
    	const writable_props = [];

    	Object.keys($$props).forEach(key => {
    		if (!~writable_props.indexOf(key) && key.slice(0, 2) !== '$$' && key !== 'slot') console.warn(`<App> was created with unknown prop '${key}'`);
    	});

    	$$self.$capture_state = () => ({
    		AppTest,
    		AppHeader,
    		PageLoader,
    		PageSelector
    	});

    	return [];
    }

    class App extends SvelteComponentDev {
    	constructor(options) {
    		super(options);
    		init(this, options, instance, create_fragment, safe_not_equal, {});

    		dispatch_dev("SvelteRegisterComponent", {
    			component: this,
    			tagName: "App",
    			options,
    			id: create_fragment.name
    		});
    	}
    }

    const app = new App({
        target: document.body,
        props: {
            name: 'world'
        }
    });

    return app;

})();
//# sourceMappingURL=bundle.js.map
