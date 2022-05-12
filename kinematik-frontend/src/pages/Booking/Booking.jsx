import React, { useState } from 'react';
import classes from './Booking.module.css';

import { useParams } from 'react-router-dom';

import { format, isEqual } from 'date-fns';
import { uk } from 'date-fns/locale';

import { capitalize } from 'lodash';
import classNames from 'classnames';

import GeneratedTicketDetail from './GeneratedTicketDetail';
import Button from '../../components/Button/Button';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faChevronLeft } from '@fortawesome/free-solid-svg-icons';

const Booking = (props) => {
    const params = useParams();

    const demoProps = {
        title: 'Spider-Man: No Way Home',
        hall: 'А',

        availableTime: [
            new Date(2022, 4, 5, 12, 30),
            new Date(2022, 4, 6, 12, 30),
            new Date(2022, 4, 7, 12, 30),
            new Date(2022, 4, 8, 12, 30),
            new Date(2022, 4, 8, 15, 30),
        ],
    };

    const years = {};
    demoProps.availableTime.forEach((availableTime) => {
        const year = availableTime.getUTCFullYear();

        if (!years[year]) {
            years[year] = {};
        }
        const correspondingYear = years[year];

        const month = availableTime.getUTCMonth();
        if (!correspondingYear[month]) {
            correspondingYear[month] = {};
        }
        const correpondingMonth = correspondingYear[month];

        const date = availableTime.getUTCDate();
        if (!correpondingMonth[date]) {
            correpondingMonth[date] = [];
        }
        const correspondingDate = correpondingMonth[date];

        correspondingDate.push(availableTime);
    });

    const availableDates = Object.keys(years).flatMap((year) => {
        return Object.keys(years[year]).flatMap((month) => {
            return Object.keys(years[year][month]).flatMap((date) => {
                return {
                    date: new Date(year, month, date),
                    availableTimes: years[year][month][date],
                };
            });
        });
    });

    const [selectedDate, setSelectedDate] = useState(availableDates[0]);
    const [selectedTime, setSelectedTime] = useState(
        selectedDate.availableTimes[0]
    );

    return (
        <div className={classes['booking-layout']}>
            <div className={classes['choosing-area']}>
                <div className={classes['top-row']}>
                    <button className={classes['back-button']}>
                        <FontAwesomeIcon icon={faChevronLeft} />
                    </button>
                    <h1 className={classes['film-title']}>{demoProps.title}</h1>
                </div>

                <div className={classes['date-choosing-area']}>
                    {availableDates.map((availableDate) => (
                        <div
                            className={classNames(
                                classes['available-date'],
                                isEqual(availableDate.date, selectedDate.date)
                                    ? classes['active']
                                    : null
                            )}
                            onClick={() => {
                                setSelectedDate(availableDate);
                            }}
                            key={availableDate.date.getTime()}
                        >
                            {capitalize(
                                format(availableDate.date, 'cccccc, d MMMM', {
                                    locale: uk,
                                })
                            )}
                        </div>
                    ))}
                </div>

                <div className={classes['time-choosing-area']}>
                    {selectedDate.availableTimes.map((availableTime) => (
                        <div
                            className={classNames(
                                classes['available-time'],
                                isEqual(availableTime, selectedTime)
                                    ? classes['active']
                                    : null
                            )}
                            onClick={() => {
                                setSelectedTime(selectedTime);
                            }}
                            key={availableTime.getTime()}
                        >
                            {capitalize(format(availableTime, 'HH:mm'))}
                        </div>
                    ))}
                </div>

                <div className={classes['seat-choosing-area']}></div>
            </div>

            <div className={classes['generated-ticket']}>
                <div
                    className={classNames(
                        classes['generated-ticket-card'],
                        classes['sidebar-card']
                    )}
                >
                    <h2 className={classes['sidebar-card-title']}>Квиток</h2>

                    <GeneratedTicketDetail
                        title="Фільм"
                        description={demoProps.title}
                    />
                    <GeneratedTicketDetail
                        title="Час"
                        description={
                            capitalize(
                                format(selectedTime, 'cccccc, d MMMM HH:mm', {
                                    locale: uk,
                                })
                            ) ?? ''
                        }
                    />
                    <GeneratedTicketDetail
                        title="Зала"
                        description={demoProps.hall}
                    />
                    <GeneratedTicketDetail
                        title="Місця"
                        // TODO Make dynamic
                        description="Б8, Б9, Б10"
                    />
                </div>
            </div>

            <div className={classes['checkout']}>
                <div
                    className={classNames(
                        classes['checkout-card'],
                        classes['sidebar-card']
                    )}
                >
                    <h2 className={classes['sidebar-card-title']}>До сплати</h2>
                    Всього: 150 грн
                    <div className={classes['checkout-buttons-container']}>
                        <Button>Купувати квиток</Button>
                        <Button variant="alternative">Бронювати</Button>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Booking;
