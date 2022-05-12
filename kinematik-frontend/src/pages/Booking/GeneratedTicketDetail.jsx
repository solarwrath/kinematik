import React from 'react';

import classes from './GeneratedTicketDetail.module.css';

const GeneratedTicketDetail = (props) => {
    return (
        <div className={classes['generated-ticket-detail']}>
            <p className={classes['generated-ticket-detail-title']}>
                {props.title}
            </p>
            <p className={classes['generated-ticket-detail-description']}>
                {props.description}
            </p>
        </div>
    );
};

export default GeneratedTicketDetail;
