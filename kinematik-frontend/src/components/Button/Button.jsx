import classNames from 'classnames';
import React from 'react';
import classes from './Button.module.css';

const Button = (props) => {
    const variant = props.variant || 'primary';
    const buttonVariantClass = `button-${variant}`;

    return (
        <button
            className={classNames(
                classes['button'],
                classes[buttonVariantClass]
            )}
            {...props}
        >
            {props.children}
        </button>
    );
};

export default Button;
